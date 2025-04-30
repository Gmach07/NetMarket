using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Errors;

namespace WebApi.Controllers
{
    [Route("api/productos")]        
    [ApiController]
    public class ProductoController : BaseApiController
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;
        public ProductoController(IGenericRepository<Producto> productoRepository, IMapper mapper) {

            _productoRepository = productoRepository;
            _mapper = mapper;   
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductoDto>>> GetProductos([FromQuery]ProductoSpecificationsParams productoParams)
        {
            var spec = new ProductoWithCategoriaAndMarcaSpecification(productoParams);


            var productos =  await _productoRepository.GetAllWithSpec(spec);

            var specCount = new ProductoForCountingSpecification(productoParams);
            var totalProductos = await _productoRepository.CountAsync(specCount);

            var rounded = Math.Ceiling( Convert.ToDecimal(totalProductos / productoParams.PageSize));
            var totalPage = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Producto>, IReadOnlyList<ProductoDto>>(productos);

            return Ok(
                  new Pagination<ProductoDto>
                  {
                      Count = totalProductos,
                      Data = data,
                      PageCount = totalPage,
                      PageIndex = productoParams.PageIndex,
                      PageSize = productoParams.PageSize
                  }
                );

            //return Ok(_mapper.Map<IReadOnlyList<Producto>, IReadOnlyList<ProductoDto>>(productos));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            //spec: debe incluir la logica de la condicion de la consulta y tambien las relaciones entre las entidades
            // la relacion entre Producto y Marca, Categoria 
            var spec = new ProductoWithCategoriaAndMarcaSpecification(id);
            var producto =  await _productoRepository.GetByIdWithSpec(spec);

            if(producto == null)
            {
                return NotFound(new CodeErrorResponse(404, "El servidor no puede encontrar la peticion"));
            }

            return _mapper.Map<Producto, ProductoDto>(producto);
            
        }



    }
}
