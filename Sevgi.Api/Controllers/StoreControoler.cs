using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Data.Services;
using Sevgi.Model;

namespace Sevgi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("store")]
    public class StoreController : ControllerBase
    {
        //This is the basic controller protected by authorization.
        //This controller uses base service injection which also uses dapper context to connect to database.

        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {

            _storeService = storeService;
        }
        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IEnumerable<Store>> GetTests()
        {
            var tests = await _storeService.GetAll();
            return tests;
        }
    }
}