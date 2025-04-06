using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repositories;

namespace Services.Filters;

public class NotFoundFilter<T>(IGenericRepository<T> genericRepository) : Attribute, IAsyncActionFilter where T : class
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // "id" parametresini al ve int'e çevir
        if (!context.ActionArguments.TryGetValue("id", out var idValue) ||
            !int.TryParse(idValue?.ToString(), out var id))
        {
            await next();
            return;
        }

        // Entity var mı kontrol et
        if (!await genericRepository.AnyAsync(id))
        {
            var entityName = typeof(T).Name;
            var endpointName = context.ActionDescriptor.RouteValues["action"];

            context.Result = new NotFoundObjectResult(
                ServiceResult.Fail($"{entityName} with ID {id} not found ({endpointName})"));
            return;
        }

        await next();
    }
}