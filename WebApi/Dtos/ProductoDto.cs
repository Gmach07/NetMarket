using Core.Entities;

namespace WebApi.Dtos
{
    public class ProductoDto
    {
        public int Id { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int Stock { get; set; }
        public int MarcaId { get; set; }

        public int CategoriaId { get; set; }

        public string MarcaNombre { get; set; }

        public string CategoriaNombre { get; set; }


        public decimal Precio { get; set; }

        public string Imagen { get; set; } = string.Empty;
    }
}
