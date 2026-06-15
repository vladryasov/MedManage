using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MedManage.Indentity.Extensions
{
    /// <summary>
    /// Расширение для настройки аутентификации Swagger с использованием JWT-токнов.
    /// </summary>
    public static class AddSwaggerAuthenticationExtension
    {
        /// <summary>
        /// Метод расширения для конфигурации Swagger с аутентификацией через JWT.
        /// </summary>
        /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
        public static void AddSwaggerAuthentication(this IServiceCollection services)
        {
            // Добавление Swagger в DI контейнер
            services.AddSwaggerGen(c =>
            {
                // Добавление определения безопасности для Bearer токена
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    // Описание для заголовка авторизации
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    
                    // Тип схемы безопасности
                    Type = SecuritySchemeType.Http,
                    
                    // Указание схемы безопасности
                    Scheme = "bearer"
                });

                // Добавление требования безопасности для использования Bearer токена
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        // Привязка схемы безопасности к методу авторизации
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                // Указание типа и идентификатора схемы безопасности
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        // Пустой массив, так как не требуется дополнительных параметров для проверки
                        Array.Empty<string>() 
                    }
                });
            });
        }
    }
}
