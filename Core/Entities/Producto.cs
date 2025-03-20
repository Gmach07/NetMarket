using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Producto : ClaseBase 
    {
        
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion {  get; set; } = string.Empty;

        public int Stock { get; set; } 
        public int MarcaId { get; set; }

        public int CategoriaId { get; set; }

        public Marca? Marca { get; set; } 

        public Categoria? Categoria {  get; set; }

       
        public decimal  Precio { get; set; }

        public string Imagen { get; set; } = string.Empty; 


    }
}
