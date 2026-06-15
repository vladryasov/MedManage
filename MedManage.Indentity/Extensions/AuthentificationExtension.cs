using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Identity.Extensions
{
    /// <summary>
    /// Расширение для добавления сервисов аутентификации через JWT.
    /// </summary>
    public static class AddAuthentificationExtension
    {
        /// <summary>
        /// Метод расширения для конфигурации аутентификации с использованием JWT.
        /// </summary>
        /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
        /// <param name="configuration">Конфигурация приложения, используемая для получения параметров JWT.</param>
        public static void AddInfrastructureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Добавление аутентификации с использованием схемы JwtBearer
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Отключение требований к использованию HTTPS (для разработки)
                options.RequireHttpsMetadata = false;

                // Сохранение токена в контексте запроса
                options.SaveToken = true;

                // Использование валидаторов токенов безопасности
                options.UseSecurityTokenValidators = true;

                // Настройки валидации токена
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Отключение проверки издателя
                    ValidateIssuer = false,

                    // Отключение проверки аудитории
                    ValidateAudience = false, 

                    // Отключение проверки срока действия токена
                    ValidateLifetime = false, 

                    // Отключение проверки подписи токена
                    ValidateIssuerSigningKey = false,

                    // Установка аудитории из конфигурации
                    ValidAudience = configuration["Jwt:Audience"],

                    // Установка издателя из конфигурации
                    ValidIssuer = configuration["Jwt:Issuer"],

                    // Установка ключа для подписи токена (секретный ключ для подписи JWT)
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "")), // Важно: не раскрывать секретные ключи JWT (предупреждение безопасности)
                   
                    NameClaimType = "sub"
                };
                
               
       

                // Настройка событий для обработки ошибок аутентификации
                options.Events = new JwtBearerEvents
                {
                    // Обработчик при неудачной аутентификации
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Токен недействителен: " + context.Exception.ToString());

                        // Проверка, что ответ еще не был отправлен
                        if (!context.Response.HasStarted)
                        {
                            // Установка статуса ответа как 401 Unauthorized
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            // Возвращение сообщения о недействительном токене
                            return context.Response.WriteAsync("Недействительный токен.");
                        }
                        else
                        {
                            // Логирование, если ответ уже был отправлен
                            Console.WriteLine("Ответ уже отправлен, нельзя установить код статуса.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
