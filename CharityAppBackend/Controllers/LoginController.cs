using System.Security.Claims;
using ActionResult;
using Auth;
using Base;
using CharityAppBL.Login;
using CharityAppBO.Login;
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
        IDLBase _dlBase;

        public LoginController(IBLLogin bLLogin, IDLBase dLBase)
        {
            _blLogin = bLLogin;
            _dlBase = dLBase;
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
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/forget-password")]
        public async Task<IActionResult> ForgetPassword(string userName)
        {
            var result = await _blLogin.ForgetPassword(userName);
            return Ok(result);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/reset-password")]
        public IActionResult ResetPassword(ResetPassword resetPassword)
        {
            var result = new ReturnResult();
            var checkResetCode = _dlBase.GetDataRedis<string>($"{resetPassword.Id}_CheckResetCode");
            if(checkResetCode != null )
            {
                var isSuccess = bool.Parse(checkResetCode);
                if(isSuccess)
                {
                    result = _blLogin.ResetPassword(resetPassword);
                    _dlBase.SaveDataRedis($"{resetPassword.Id}_CheckResetCode", "false", null);
                    return Ok(result);    
                    
                }
            }
            result.BadRequest(new List<string> { "Nhập đúng mã trước khi thay đổi mật khẩu" });
            return BadRequest(result);
            
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/reset-code")]
        public IActionResult ResetCode(ResetCode resetCode)
        {
            var result = _blLogin.ResetCode(resetCode);
            if (result.IsSuccess)
            {
                _dlBase.SaveDataRedis($"{resetCode.Id}_CheckResetCode", "true", null);
                return Ok(result);
            }
            else
            {
                _dlBase.SaveDataRedis($"{resetCode.Id}_CheckResetCode", "false", null);
                return BadRequest(result);
            }
        }
    }
}
