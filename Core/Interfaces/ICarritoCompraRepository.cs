using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICarritoCompraRepository
    {
        Task<CarritoCompra> GetCarritoCompraAsync(string carritoId);
        Task<CarritoCompra> UpdateCarritoCompraAsync(CarritoCompra carrito);
        Task<bool> DeleteCarritoCompraAsync(string carritoId);
        Task<bool> ClearCarritoCompraAsync(string carritoId);
        Task<bool> AddItemToCarritoAsync(string carritoId, CarritoItem item);
        Task<bool> RemoveItemFromCarritoAsync(string carritoId, int itemId);
    }
}
