namespace MedManage.Persistence.Caching
{
    using System.Reflection;
    using System.Collections.Concurrent;
    using Castle.DynamicProxy;
    using MedManage.Persistence.Caching;
    using Microsoft.Extensions.Logging;

    public class CachingInterceptor : IInterceptor
    {
        private readonly IInMemoryCache _cache;
        private readonly ILogger<CachingInterceptor> _logger;

        // Кеш для готовых generic-методов InterceptAsyncWithCache<T>
        private static readonly ConcurrentDictionary<Type, MethodInfo> _cachedAsyncHandlers = new();

        public CachingInterceptor(IInMemoryCache cache, ILogger<CachingInterceptor> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttr = GetCacheAttribute(invocation);
            var invalidateAttr = GetInvalidateAttribute(invocation);

            if (invalidateAttr != null)
            {
                InvalidateCache(invocation, invalidateAttr);
            }

            if (cacheAttr == null)
            {
                invocation.Proceed();
                return;
            }

            var cacheKey = BuildKey(cacheAttr.KeyTemplate, invocation);
            var returnType = invocation.Method.ReturnType;

            // Для async Task<T>
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GenericTypeArguments[0];
                var method = GetOrCreateAsyncCacheMethod(resultType);
                invocation.ReturnValue = method.Invoke(this, new object[] { invocation, cacheKey, cacheAttr.ExpirationSeconds });
                return;
            }

            // Для синхронных методов
            if (_cache.TryGet(cacheKey, out object? cachedValue))
            {
                _logger.LogInformation("Cache HIT for key: {Key}", cacheKey);
                invocation.ReturnValue = cachedValue;
                return;
            }

            _logger.LogInformation("Cache MISS for key: {Key}", cacheKey);
            invocation.Proceed();
            var result = invocation.ReturnValue;
            if (result != null)
                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(cacheAttr.ExpirationSeconds));
        }

        private async Task<T> InterceptAsyncWithCache<T>(IInvocation invocation, string cacheKey, int expirationSeconds)
        {
            if (_cache.TryGet<T>(cacheKey, out var cached))
            {
                _logger.LogInformation("Cache HIT for key: {Key}", cacheKey);
                return cached!;
            }

            _logger.LogInformation("Cache MISS for key: {Key}", cacheKey);
            invocation.Proceed();
            var task = (Task<T>)invocation.ReturnValue!;
            var result = await task;
            if (result != null)
                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(expirationSeconds));
            return result;
        }

        private void InvalidateCache(IInvocation invocation, CacheInvalidateAttribute attr)
        {
            foreach (var template in attr.KeyTemplates)
            {
                var key = BuildKey(template, invocation);
                _cache.Remove(key);
                _logger.LogDebug("Invalidated cache key: {Key}", key);
            }
        }

        private string BuildKey(string template, IInvocation invocation)
        {
            // Замена {имя_параметра} на значение аргумента
            var parameters = invocation.Method.GetParameters();
            var args = invocation.Arguments;
            var result = template;
            for (int i = 0; i < parameters.Length; i++)
            {
                result = result.Replace($"{{{parameters[i].Name}}}", args[i]?.ToString() ?? "null");
            }
            // Можно добавить имя типа и метода, чтобы избежать коллизий
            return $"{invocation.TargetType?.Name}.{invocation.Method.Name}:{result}";
        }

        private static CacheAttribute? GetCacheAttribute(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget?.GetCustomAttribute<CacheAttribute>(inherit: true)
                ?? invocation.Method.GetCustomAttribute<CacheAttribute>(inherit: true);
        }

        private static CacheInvalidateAttribute? GetInvalidateAttribute(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget?.GetCustomAttribute<CacheInvalidateAttribute>(inherit: true)
                ?? invocation.Method.GetCustomAttribute<CacheInvalidateAttribute>(inherit: true);
        }

        private static MethodInfo GetOrCreateAsyncCacheMethod(Type resultType)
        {
            return _cachedAsyncHandlers.GetOrAdd(resultType, static type =>
            {
                var method = typeof(CachingInterceptor)
                    .GetMethod(nameof(InterceptAsyncWithCache), BindingFlags.NonPublic | BindingFlags.Instance)!;
                return method.MakeGenericMethod(type);
            });
        }
    }
}