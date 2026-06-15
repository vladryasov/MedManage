using System.Data;
using System.Reflection;
using System.Collections.Concurrent;
using Castle.DynamicProxy;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Transactions;

public class TransactionInterceptor : IInterceptor
{
    private readonly IAppDbContext _context;

    // Кеш для готовых generic-методов InterceptAsyncWithResult<T>
    private static readonly ConcurrentDictionary<Type, MethodInfo> _cachedAsyncHandlers = new();

    public TransactionInterceptor(IAppDbContext context)
    {
        _context = context;
    }

    public void Intercept(IInvocation invocation)
    {
        var attribute = GetTransactionalAttribute(invocation);
        if (attribute is null)
        {
            invocation.Proceed();
            return;
        }

        // Если уже есть активная транзакция, не создаём новую
        if (HasActiveTransaction())
        {
            invocation.Proceed();
            return;
        }

        var returnType = invocation.Method.ReturnType;

        if (returnType == typeof(Task))
        {
            invocation.ReturnValue = InterceptAsync(invocation, attribute.IsolationLevel);
            return;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = returnType.GenericTypeArguments[0];
            var method = GetOrCreateAsyncHandler(resultType);
            invocation.ReturnValue = method.Invoke(this, new object[] { invocation, attribute.IsolationLevel });
            return;
        }

        InterceptSync(invocation, attribute.IsolationLevel);
    }

    private bool HasActiveTransaction()
    {
        // Пытаемся получить текущую транзакцию из DbContext
        return (_context as DbContext)?.Database.CurrentTransaction != null;
    }

    private static TransactionalAttribute? GetTransactionalAttribute(IInvocation invocation)
    {
        return invocation.MethodInvocationTarget?.GetCustomAttribute<TransactionalAttribute>(inherit: true)
            ?? invocation.Method.GetCustomAttribute<TransactionalAttribute>(inherit: true);
    }

    private void InterceptSync(IInvocation invocation, IsolationLevel isolationLevel)
    {
        using var transaction = _context.BeginTransaction(isolationLevel);

        try
        {
            invocation.Proceed();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task InterceptAsync(IInvocation invocation, IsolationLevel isolationLevel)
    {
        await using var transaction = await _context.BeginTransactionAsync(isolationLevel);

        try
        {
            invocation.Proceed();
            await ((Task)invocation.ReturnValue!).ConfigureAwait(false);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<T> InterceptAsyncWithResult<T>(IInvocation invocation, IsolationLevel isolationLevel)
    {
        await using var transaction = await _context.BeginTransactionAsync(isolationLevel);

        try
        {
            invocation.Proceed();
            var result = await ((Task<T>)invocation.ReturnValue!).ConfigureAwait(false);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Получение или создание сконструированного метода InterceptAsyncWithResult<T> для заданного типа T
    private static MethodInfo GetOrCreateAsyncHandler(Type resultType)
    {
        return _cachedAsyncHandlers.GetOrAdd(resultType, static type =>
        {
            var method = typeof(TransactionInterceptor)
                .GetMethod(nameof(InterceptAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic)!;
            return method.MakeGenericMethod(type);
        });
    }
}