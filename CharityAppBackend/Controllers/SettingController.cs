using ActionResult;
using CharityAppBL.Setting;
using CharityAppBO.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CharityAppBackend.Controllers
{
    [Route("setting")]
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
        [HttpPut("user-normal/edit-info/{id}")]
        [Authorize(Roles = "UserNormal")]
        public async Task<IActionResult> UpdateUserNormal(int id, [FromBody] UserNormalUpdate userNormalUpdate)
        {
            var oldPassword = userNormalUpdate.OldPassword;
            var result = new ReturnResult();
            if (String.IsNullOrEmpty(oldPassword))
            {
                // ko co password => update info
                result = await _bLSetting.UpdateInfo(id, false, userNormalUpdate);
            }
            else
            {
                // update password (co the co info hoac khong)
                result = await _bLSetting.UpdateInfo(id, true, userNormalUpdate);

            }
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPut("user-charity/edit-info/{id}")]
        [Authorize(Roles = "UserCharity")]
        public async Task<IActionResult> UpdateCharityInfo(int id, [FromBody] UserCharityUpdate userCharityUpdate)
        {
            var result = await _bLSetting.UpdateCharityInfo(id, userCharityUpdate);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("user-charity/edit-password/{id}")]
        [Authorize(Roles = "UserCharity")]
        public IActionResult UpdateCharityPassword(int id, [FromBody] UpdatePassword updatePassword)
        {
            var result = _bLSetting.UpdateCharityPassword(id, updatePassword);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
