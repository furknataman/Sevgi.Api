using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Api.Infrastructure.RequestModels;
using ProjectBase.Data.Services;
using ProjectBase.Model;

namespace ProjectBase.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
 
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        {
            var response = await _authService.SignIn(request.Email, request.Password);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
        {
            var response = await _authService.SignUp(request.Email, request.Password);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("external")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExternalAuth([FromBody] AuthRequest request)
        {
            var result = await _authService.ExternalAuth(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("clear-users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClearUsers()
        {
            await _authService.ClearUsers();
            return Ok();
        }
    }
}
