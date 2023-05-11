
using CharityAppBL.Charity;
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
        public ChairityController(IBLCharity iBLCharity)
        {
            _blCharity = iBLCharity;
        }
        // GET: api/<ChairityController>
        [HttpGet("all")]
        public IActionResult GetALlCharities()
        {
            var result = _blCharity.GetAllCharities();
            return Ok(result);
        }
    }
}
