using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Api.Infrastructure.RequestModels;
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
        public async Task<IEnumerable<Store>> GetAllStore()
        {
            var tests = await _storeService.GetAll();
            return tests;
        }


        [AllowAnonymous]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateStore([FromBody] Store store)
        {
            var tests = await _storeService.Update(store);
            return Ok(tests);

        }
        
      
        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<IEnumerable<Store>>AddStore (Store store)
        {
            var tests = await _storeService.Add(store);
            return tests;
        }
    }
}