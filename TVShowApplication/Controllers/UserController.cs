﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVShowApplication.Data.DTO.User;
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
            var tokens = await _userManager.GetTokenForUser(signInRequest);

            if (tokens == null) return Unauthorized();

            return Ok(tokens);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("token/refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshRequest)
        {
            var authentication = await _userManager.RefreshToken(refreshRequest);

            if (authentication == null) return Unauthorized();

            return Ok(authentication);
        }

        [HttpPost]
        [Authorize]
        [Route("token/revoke")]
        public async Task<IActionResult> RevokeToken()
        {
            await _userManager.Revoke();

            return Ok();
        }
    }
}
