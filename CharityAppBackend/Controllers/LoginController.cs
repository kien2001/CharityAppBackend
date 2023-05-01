using System.Security.Claims;
using ActionResult;
using Auth;
using CharityAppBL.Login;
using Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        IBLLogin _blLogin;

        public LoginController(IBLLogin bLLogin)
        {
            _blLogin = bLLogin;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserLogin user)
        {
            ReturnResult result = _blLogin.Authenthicate(user);
            return Ok(result);
        }

        //[HttpPost]
        //[Authorize]
        //public IActionResult Logout()
        //{

        //    return Ok();
        //}

        //[HttpPost]
        //[Route("refreshtoken")]
        //public IActionResult RefreshToken([FromBody] TokenRequest tokenRequest)
        //{
        //    ReturnResult returnResult = _blLogin.VerifyToken(tokenRequest);

        //    if (returnResult.IsSuccess)
        //    {
        //        return Ok(returnResult);
        //    }else if(!returnResult.IsAuthorized)
        //    {
        //        return Unauthorized(returnResult);
        //    }
        //    return BadRequest(returnResult);
        //}

    }
}
