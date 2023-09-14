using System.Net;

namespace Triagem.Erros
{
    public class AuthorizationErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                // Tratar erros 401 aqui
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }


}
