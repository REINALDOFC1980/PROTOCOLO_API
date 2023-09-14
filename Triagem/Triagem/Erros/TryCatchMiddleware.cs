using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class TryCatchMiddleware
{
    private readonly RequestDelegate _next;

    public TryCatchMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Chama o próximo middleware na pipeline.
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";

            if (ex is ApplicationException || ex is InvalidOperationException)
            {
                // Manipule exceções específicas que resultam em status 400 (BadRequest).
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                // Todas as outras exceções serão tratadas como status 500 (InternalServerError).
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            string mensagemDeErro = ex.Message; // Use a mensagem de erro da exceção ou qualquer outra mensagem personalizada.

            var errorResponse = new
            {
                erro = context.Response.StatusCode,
                mensagem = mensagemDeErro
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
