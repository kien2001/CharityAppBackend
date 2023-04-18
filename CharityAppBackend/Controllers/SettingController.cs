using CharityAppBL.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CharityAppBackend.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly IBLSetting _bLSetting;

        public SettingController(IBLSetting bLSetting) 
        {
            _bLSetting = bLSetting;
        }
        [HttpPost("CheckPassword/{id}/{password}")]
        public IActionResult CheckPassword(int id, string password)
        {
            var result = _bLSetting.CheckPassword(id, password);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("Update/{id}/{roleId}/UpdatePassword={isUpdatePassword}")]
        public IActionResult UpdateInfo(int id, int roleId, string isUpdatePassword, [FromBody] object userUpdate)
        {
            var result = _bLSetting.UpdateInfo(id, roleId, isUpdatePassword, userUpdate);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
