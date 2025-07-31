namespace WebApi.Dtos
{
    public class ActualizarDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        
        public string? Password { get; set; }

        public string? Imagen { get; set; }
    }
}
