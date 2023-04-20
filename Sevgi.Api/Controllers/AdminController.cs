using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Data.Services;
using Sevgi.Model;

namespace Sevgi.Api.Controllers
{
    //[Authorize(Roles = "ADMINISTRATOR")]
    [ApiController]
    [Authorize]
    [Route("admin")]
    public class AdminControlller : ControllerBase
    {
        //This is the basic controller protected by authorization.
        //This controller uses base service injection which also uses dapper context to connect to database.

        private readonly IAdminService _adminService;
        private readonly ICardService _cardService;

        public AdminControlller(IAdminService adminService, ICardService cardService)
        {
            _adminService = adminService;
            _cardService = cardService;

        }

        //[Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-users")]
        public async Task<IEnumerable<UserView>> GetUsers()
        {
            var tests = await _adminService.GetAll();
            return tests;
        }

        //[Authorize(Roles = "CASHIER")]
        //[Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-sales")]
        public async Task<IEnumerable<Sale>> GetSales()
        {
            var tests = await _adminService.GetAllSell();
            return tests;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var response = await _adminService.UpdateUser(request);
            if (response.Succeeded) return Ok(response);
            else return BadRequest(response);
        }

        [HttpGet("card-control")]
        public async Task<int> CardControl(string cardNo)
        {
            var tests = await _cardService.CardControl(cardNo);
            return tests;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest newUser, string cardNumber)
        {
            await _adminService.CreateUser(newUser, cardNumber);
            return Ok();
        }
    }
}