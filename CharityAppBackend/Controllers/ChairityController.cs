
using CharityAppBL.Charity;
using CharityAppBL.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CharityAppBackend.Controllers
{
    [Route("charity")]
    //[Authorize]
    [ApiController]
    public class ChairityController : ControllerBase
    {
        private IBLCharity _blCharity;
        private IBLAccount _blAccount;

        public ChairityController(IBLCharity iBLCharity, IBLAccount ibLAccount)
        {
            _blCharity = iBLCharity;
            _blAccount = ibLAccount;
        }
        // GET: api/<ChairityController>
        [HttpGet("all")]
        public IActionResult GetALlCharities()
        {
            var result = _blCharity.GetAllCharities();
            return Ok(result);
        }
        [HttpGet("{charityId}")]
        public IActionResult GetCharity(int charityId)
        {
            var result = _blAccount.GetUser(charityId);
            return Ok(result);
        }
    }
}
