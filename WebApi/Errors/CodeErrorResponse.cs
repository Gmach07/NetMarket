namespace WebApi.Errors
{
    public class CodeErrorResponse
    {
        
        public CodeErrorResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefautMessageStatusCode(statusCode);
        }

        private string GetDefautMessageStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "El request tiene errores",
                401 => "No tienes permiso para ese error",
                404 => "El recurso no se encuentra disponible",
                500 => "Se produjeron errores en el servidor",
                _ => null
            };
        }

        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
