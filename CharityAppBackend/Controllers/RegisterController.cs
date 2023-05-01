using ActionResult;
using CharityAppBL.Login;
using Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CharityAppBackend.Controllers
{
    [Route("register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        IBLLogin _blLogin;

        public RegisterController(IBLLogin bLLogin)
        {
            _blLogin = bLLogin;
        }
        [HttpPost("")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserRegister userRegister)
        {
            ReturnResult returnResult = _blLogin.Register(userRegister);
            return Ok(returnResult);
        }
    }
}
