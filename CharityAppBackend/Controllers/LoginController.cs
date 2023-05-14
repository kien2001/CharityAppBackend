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
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            ReturnResult result = await _blLogin.Authenthicate(user);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("/logout")]
        public IActionResult Logout()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = _blLogin.Logout(id);
            return Ok();
        }

        [HttpGet("forget-password")]
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            var a = CharityUtil.SendResetPasswordEmail("kienlevan2001@gmail.com", "9285783");
            return Ok(a);
        }
    }
}
