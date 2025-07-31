// NetMarket/WebApi/Errors/CodeErrorResponse.cs
using System;

namespace WebApi.Errors
{
    public class CodeErrorResponse
    {
        public CodeErrorResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Has realizado una solicitud incorrecta",
                401 => "No estas autorizado para este recurso",
                404 => "Recurso no encontrado",
                500 => "Error interno del servidor",
                _ => null
            };
        }
    }
}