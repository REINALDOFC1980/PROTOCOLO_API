using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

public class CustomAuthorizeFilter : IAuthorizationFilter
{
    private readonly string[] _roles;

    public CustomAuthorizeFilter(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string errorMessage = null;
        int statusCode = 0;

        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            errorMessage = "Você não está autenticado.";
            statusCode = 401;
        }
        else if (_roles != null && _roles.Length > 0 && !_roles.Any(role => context.HttpContext.User.IsInRole(role)))
        {
            errorMessage = "Acesso negado.";
            statusCode = 403;
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            var errorResponse = new
            {
                Erro = statusCode,
                Message = errorMessage
            };

            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = statusCode
            };

            return;
        }
    }
}
