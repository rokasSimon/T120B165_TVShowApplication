using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Data.DTO;

namespace TVShowApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        

        [HttpPost]
        [AllowAnonymous]
        [Route("/token")]
        public async Task<IActionResult> GetToken(SignInRequest signInRequest)
        {
            return Ok();
        }
    }
}
