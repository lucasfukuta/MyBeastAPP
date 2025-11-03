using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBeast.API.Middleware
{
    // Middleware para capturar todas as exceções da aplicação
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger; // Para a Etapa 3

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Tenta executar o controller
            }
            catch (Exception ex)
            {
                // Se um erro ocorrer, captura e trata
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 (Padrão)
            var errorMessage = "Ocorreu um erro interno no servidor."; // Mensagem segura

            // Personaliza a resposta com base no tipo de exceção
            // (Isso é seguro porque são nossas próprias mensagens de exceção)
            if (exception is ArgumentException || exception.Message.Contains("inválido"))
            {
                code = HttpStatusCode.BadRequest; // 400
                errorMessage = exception.Message;
            }
            else if (exception.Message.Contains("não encontrado"))
            {
                code = HttpStatusCode.NotFound; // 404
                errorMessage = exception.Message;
            }
            else if (exception.Message.Contains("permissão"))
            {
                code = HttpStatusCode.Forbidden; // 403
                errorMessage = exception.Message;
            }
            // (Adicionar mais casos se necessário)

            // Loga o erro real (Etapa 3)
            _logger.LogError(exception, "Erro capturado pelo Middleware: {message}", errorMessage);

            // Cria a resposta JSON padronizada
            var result = JsonSerializer.Serialize(new { error = errorMessage });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }
}