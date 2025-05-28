using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using WebApi.Errors;

namespace WebApi.Controllers
{
  
    public class CarritoCompraController : BaseApiController
    {
      private readonly ICarritoCompraRepository _carritoCompraRepository;
        public CarritoCompraController(ICarritoCompraRepository carritoCompraRepository)
        {
            _carritoCompraRepository = carritoCompraRepository;
        }
        [HttpGet("{carritoId}")]
        public async Task<ActionResult<CarritoCompra>> GetCarritoById(string carritoId)
        {
            var carrito = await _carritoCompraRepository.GetCarritoCompraAsync(carritoId);
            if (carrito == null) return NotFound(new CodeErrorResponse(404));
            return Ok(carrito ?? new CarritoCompra(carritoId));
        }

        [HttpPost]
        public async Task<ActionResult<CarritoCompra>> UpdateCarritoCompra([FromBody] CarritoCompra carrito)
        {
            if (carrito == null) return BadRequest(new CodeErrorResponse(400));
            var updatedCarrito = await _carritoCompraRepository.UpdateCarritoCompraAsync(carrito);
            if (updatedCarrito == null) return NotFound(new CodeErrorResponse(404));
            return Ok(updatedCarrito);
        }


        [HttpDelete("{carritoId}")]
        public async Task<ActionResult<bool>> DeleteCarrito(string carritoId)
        {
            var result = await _carritoCompraRepository.DeleteCarritoCompraAsync(carritoId);
            if (!result) return NotFound(new CodeErrorResponse(404));
            return Ok(result);
        }

    }
}
