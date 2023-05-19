
using ActionResult;
using CharityAppBL.Charity;
using CharityAppBL.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CharityAppBackend.Controllers
{
    [Route("charity")]
    [AllowAnonymous]
    [ApiController]
    public class CharityController : ControllerBase
    {
        private IBLCharity _blCharity;
        private IBLAccount _blAccount;

        public CharityController(IBLCharity iBLCharity, IBLAccount ibLAccount)
        {
            _blCharity = iBLCharity;
            _blAccount = ibLAccount;
        }
        // GET: api/<ChairityController>
        [HttpGet("all")]
        public IActionResult GetALlCharities()
        {
            var result = new ReturnResult();
            try
            {
                var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                result = _blCharity.GetAllCharities(id);
            }
            catch (Exception e)
            {
                result = _blCharity.GetAllCharities(null);
            }
            return Ok(result);
        }
        [HttpGet("{charityId}")]
        public IActionResult GetCharity(int charityId)
        {
            var result = new ReturnResult();
            try
            {
                var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                result = _blCharity.GetCharityById(charityId, id);
            }
            catch (Exception e)
            {
                result = _blCharity.GetCharityById(charityId, null);
            }
            return Ok(result);
        }

        [HttpPost("UploadVerification")]
        public IActionResult UploadVerification(List<IFormFile> files, string subDirectory)
        {
            var result = new ReturnResult();
            
            return Ok(result);
        }
    }
}
