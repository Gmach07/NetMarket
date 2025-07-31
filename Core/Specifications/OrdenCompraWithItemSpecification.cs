using Core.Entities.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrdenCompraWithItemSpecification : BaseSpecification<OrdenCompras>
    {
        public OrdenCompraWithItemSpecification(string email) : base(o => o.CompradorEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.TipoEnvio);
            AddOrderByDescending(o => o.OrdenCompraFecha);
        }

        public OrdenCompraWithItemSpecification(int id, string email)
            : base(o => o.CompradorEmail == email && o.Id == id)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.TipoEnvio);
            AddOrderByDescending(o => o.OrdenCompraFecha);
        }
    }
}


