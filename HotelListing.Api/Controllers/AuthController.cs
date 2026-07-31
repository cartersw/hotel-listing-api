using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{
    public class AuthController : ApiControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
