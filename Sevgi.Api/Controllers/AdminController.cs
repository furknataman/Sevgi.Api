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
        [AllowAnonymous]
        [HttpGet("get-users")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            var tests = await _adminService.GetAll();
            return tests;
        }

        [AllowAnonymous]
        [HttpPost("update")]
        public async Task UpdateUser( string id, string name, string surname, string telephoneNuber, bool status)
        {
          var response=await _adminService.Update(id: id,name: name, surname:surname, telephoneNuber: telephoneNuber, status: status);

        }
    }
}