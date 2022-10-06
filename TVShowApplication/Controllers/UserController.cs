using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Services.Database;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            var success = await _userManager.CreateUser(signUpRequest);

            if (!success) return BadRequest();

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("token")]
        public async Task<IActionResult> GetToken(SignInRequest signInRequest)
        {
            var jwtToken = await _userManager.GetTokenForUser(signInRequest);

            if (jwtToken == null) return BadRequest();

            return Ok(jwtToken);
        }
    }
}
