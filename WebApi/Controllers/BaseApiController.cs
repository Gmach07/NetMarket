// NetMarket/WebApi/Controllers/BaseApiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Asegúrate de que esto esté importado

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // <--- ¡ASEGÚRATE DE QUE ESTO ESTÉ AQUÍ!
    public class BaseApiController : ControllerBase
    {
    }
}
