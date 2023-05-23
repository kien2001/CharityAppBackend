using CharityAppBL.Users;
using CharityAppBO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CharityAppBackend.Controllers
{
    [Route("user")]
    //[Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IBLUser _blUser;
        public UserController(IBLUser iBLUser) { 
            _blUser = iBLUser;
            
        }

        [HttpGet("charity-follow/{userId}")]
        //[Authorize(Roles = "UserNormal")]
        public IActionResult GetFollowChairties(int userId)
        {
            var result = _blUser.GetFollowCharities(userId);
            return Ok(result);
        }

        [HttpPost("charity-follow/change-status")]
        //[Authorize(Roles = "UserNormal")]
        public IActionResult ChangeStatusFollow(StatusFollow statusFollow)
        {
            var result = _blUser.ChangeStatusFollow(statusFollow);
            return Ok(result);
        }

    }
}
