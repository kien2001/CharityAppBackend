using CharityAppBL.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharityAppBackend.Controllers
{
    [Route("upload")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IBLUpload _bLUpload;
        public UploadController(IBLUpload bLUpload) {
            _bLUpload = bLUpload;
        }
        [HttpPost]
        public async Task<ActionResult<string>> UploadFile(IFormFile file)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _bLUpload.UploadFile(id, file);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
