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
    [Route("[controller]")]
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

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserRegister user)
        {
            ReturnResult returnResult = _blLogin.Register(user);
            return Ok(returnResult);
        }

        [HttpPost]
        [Route("refreshtoken")]
        public IActionResult RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            ReturnResult returnResult = _blLogin.VerifyToken(tokenRequest);

            if (returnResult.IsSuccess)
            {
                return Ok(returnResult);
            }else if(!returnResult.IsAuthorized)
            {
                return Unauthorized(returnResult);
            }
            return BadRequest(returnResult);
        }

    }
}
