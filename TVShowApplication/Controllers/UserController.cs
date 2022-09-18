using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Data.DTO;

namespace TVShowApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("/get-token")]
        public async Task<IActionResult> GetToken(SignInRequest signInRequest)
        {
            return Ok();
        }
    }
}
