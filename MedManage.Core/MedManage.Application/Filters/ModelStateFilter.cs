using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedManage.Application.Filters;

/// <summary>
/// Атрибут для проверки валидности модели перед выполнением действия контроллера.
/// </summary>
public class ValidateModelStateAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Переопределённый метод для проверки состояния модели.
    /// </summary>
    /// <param name="context">Контекст выполнения действия.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}