using Core.Entities;
using Core.Entities.OrdenCompra;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly ICarritoCompraRepository _carritoCompraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrdenCompraService(ICarritoCompraRepository carritoCompraRepository, IUnitOfWork unitOfWork)
        {
            _carritoCompraRepository = carritoCompraRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Core.Entities.OrdenCompra.Direccion direccion)
        {
            var carritoCompra = await _carritoCompraRepository.GetCarritoCompraAsync(carritoId);

            var items = new List<OrdenItem>();
            foreach (var item in carritoCompra.Items)
            {
                var productoItem = await _unitOfWork.Repository<Producto>().GetByIdAsync(item.Id);
                var itemOrdenado = new ProductoItemOrdenado(productoItem.Id, productoItem.Nombre, productoItem.Imagen);
                var ordenItem = new OrdenItem(itemOrdenado, productoItem.Precio, item.Cantidad);
                items.Add(ordenItem);
            }

            var tipoEnvioEntity = await _unitOfWork.Repository<TipoEnvio>().GetByIdAsync(tipoEnvio);

            var subtotal = items.Sum(item => item.Precio * item.Cantidad);

            var ordenCompra = new OrdenCompras(compradorEmail, direccion, tipoEnvioEntity, items, subtotal);

            _unitOfWork.Repository<OrdenCompras>().AddEntity(ordenCompra);

            var resultado = await _unitOfWork.Complete();

            if (resultado <= 0)
            {
                return null;
            }

            await _carritoCompraRepository.DeleteCarritoCompraAsync(carritoId);

            return ordenCompra;
        }

        public async Task<OrdenCompras> GetOrdenComprasByIdAsync(int id, string email)
        {
            var spec = new OrdenCompraWithItemSpecification(id, email);

            return await _unitOfWork.Repository<OrdenCompras>().GetByIdWithSpec(spec);
        }

        public async Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email)
        {
            var spec = new OrdenCompraWithItemSpecification(email);

            return await _unitOfWork.Repository<OrdenCompras>().GetAllWithSpec(spec);
        }

        public async Task<IReadOnlyList<TipoEnvio>> GetTipoEnvios()
        {
            return await _unitOfWork.Repository<TipoEnvio>().GetAllAsync();
        }
    }
}
