using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Errors;

namespace WebApi.Controllers
{
    [Route("errors")]
    
    public class ErrorController : BaseApiController
    {
        public IActionResult Error(int Code)
        {
            return new ObjectResult (new CodeErrorResponse(Code));
        }
    }
}
