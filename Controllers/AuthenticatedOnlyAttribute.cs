using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthenticatedOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var email = context.HttpContext.Session.GetString("MonkeyEmail");
        if (string.IsNullOrEmpty(email))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}
