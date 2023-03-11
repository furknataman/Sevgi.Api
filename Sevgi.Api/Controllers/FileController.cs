using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevgi.Data.Services;
using Sevgi.Model;

namespace Sevgi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("file")]
    public class FileController : ControllerBase
    {
        //This is the basic controller protected by authorization.
        //This controller uses base service injection which also uses dapper context to connect to database.
        private readonly ILogger<BaseController> _logger;
        private readonly IBaseService _baseService;

        private readonly IUtilService _utilService;

        public FileController( IUtilService utilService)
        {
            _utilService = utilService;
        }
     

        [AllowAnonymous]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            byte[] fileData;
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                fileData = target.ToArray();
            }

            var fileToUpload = new UploadableFile()
            {
                Name = "dene",
                Data = fileData,
                Type = "de"
            };
            var tests = await _utilService.uploadFile(fileToUpload);
            return Ok(tests);
        }

        [AllowAnonymous]
        [HttpGet("download-image")]
        public async Task<IActionResult> DownLoadImage( int id)
        {
           
            var tests = await _utilService.DownloadFile(id);
            return Ok(tests);
        }
    }
}