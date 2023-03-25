using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Data.Services;
using Sevgi.Model;
using System.Security.Claims;

namespace Sevgi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        //This is the basic controller protected by authorization.
        //This controller uses base service injection which also uses dapper context to connect to database.
        private readonly UserManager<User> _userManager;
        private readonly IProfileService _profileService;

        public ProfileController(UserManager<User> userManager, IProfileService profileService)
        {
            _userManager = userManager;
            _profileService = profileService;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfile(ProfileInformation request)
        {
            //get the authenticated user
            var user = await GetCurrentUserAsync();
            if (user is null) return BadRequest();

            await _profileService.Update(user, request);
            return Ok();
        }

        [HttpGet("claim-card")]
        public async Task<IActionResult> ClaimCard(string id)
        {
            //get the authenticated user
           var user = await GetCurrentUserAsync();


            


           if (user is null) return BadRequest();

            await _userManager.DeleteAsync(user);

            await _profileService.ClaimCard(user.Id);
            return Ok();
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            //security first
            var email = User.FindFirst(ClaimTypes.Email)!.Value;
            if (email is null) return null;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return user;
        }


        [HttpGet("info")]
        public async Task<IActionResult> InfoProfile()
        {
            //get the authenticated user
            var user = await GetCurrentUserAsync();
           if (user is null) return BadRequest();

            var result = await _profileService.GetUserInfo(user.Id);
            return Ok(result);
        }


        [HttpGet("info-sale")]
        public async Task<IActionResult> InfoUserSale(String id)
        {
            //get the authenticated user
            var result = await _profileService.GetUserSale( id);
            return Ok(result);
        }
    }
}