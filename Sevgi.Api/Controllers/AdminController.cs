using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Data.Services;
using Sevgi.Model;

namespace Sevgi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("admin")]
    public class AdminControlller : ControllerBase
    {
        //This is the basic controller protected by authorization.
        //This controller uses base service injection which also uses dapper context to connect to database.

        private readonly IAdminService _adminService;

        public AdminControlller(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("get-users")]
        public async Task<IEnumerable<UserView>> GetUsers()
        {
            var tests = await _adminService.GetAll();
            return tests;
        }

        [HttpGet("get-sales")]
        public async Task<IEnumerable<Sale>> getSales()
        {
            var tests = await _adminService.GetAllSell();
            return tests;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
        {
            var response = await _adminService.UpdateUser(request.Id,request.Name ,request.Surname,request.phoneNumber,request.Status);
            if (response.Succeeded) return Ok(response);
            else return BadRequest(response);
        }
    }
}