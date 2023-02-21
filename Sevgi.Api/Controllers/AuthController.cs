using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Api.Infrastructure.RequestModels;
using Sevgi.Data.Services;
using Sevgi.Model;
using System.Security.Claims;

namespace Sevgi.Api.Controllers
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
        [HttpPost("external")]
        public async Task<IActionResult> ExternalAuth([FromBody] AuthRequest request)
        {
            var result = await _authService.ExternalAuth(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("clear-users")]
        public async Task<IActionResult> ClearUsers()
        {
            await _authService.ClearUsers();
            return Ok();
        }
    }
}
