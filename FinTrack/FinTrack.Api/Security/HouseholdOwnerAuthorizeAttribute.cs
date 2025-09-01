using FinTrack.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinTrack.Api.Security
{

    [AttributeUsage(AttributeTargets.Method)]
    public class HouseholdOwnerAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            BaseController? controller = context.Controller as BaseController;
            Guid? userId = controller?.GetUserId();

            if (!context.ActionArguments.TryGetValue("householdId", out object? householdIdObj) || userId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (householdIdObj is not Guid householdId)
            {
                context.Result = new ForbidResult();
                return;
            }

            IHouseholdService householdService = context.HttpContext.RequestServices.GetRequiredService<IHouseholdService>();
            bool userIsOwner = await householdService.IsUserHouseholdOwner(userId.Value, householdId);
            if (!userIsOwner)
            {
                context.Result = new ForbidResult("User cannot perform this action");
                return;
            }

            await next();
        }
    }
}
