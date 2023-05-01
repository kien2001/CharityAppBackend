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
        [Authorize(Roles = "UserNormal")]
        public IActionResult CheckPassword(int id, string password)
        {
            var result = _bLSetting.CheckPassword(id, password);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateUser/{roleId}/{id}")]
        [Authorize(Roles = "UserCharity")]
        public IActionResult UpdateUserNormal(int id, int roleId, [FromBody] UserNormalUpdate userNormalUpdate)
        {
            var oldPassword = userNormalUpdate.OldPassword;
            var result = new ReturnResult();
            if (String.IsNullOrEmpty(oldPassword))
            {
                // ko co password => update info
                result = _bLSetting.UpdateInfo<UserNormalUpdate>(id, roleId, false, userNormalUpdate);


            }
            else
            {
                // update info (co the co password hoac khong)
                result = _bLSetting.UpdateInfo<UserNormalUpdate>(id, roleId, true, userNormalUpdate);

            }
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("UpdateCharity/{roleId}/{id}")]
        public IActionResult UpdateCharity(int id, int roleId, [FromBody] UserCharityUpdate userCharityUpdate)
        {
            var result = new ReturnResult();
            var oldPassword = userCharityUpdate.OldPassword;
            if (String.IsNullOrEmpty(oldPassword))
            {
                // ko co password => update info
                result = _bLSetting.UpdateInfo<UserCharityUpdate>(id, roleId, false, userCharityUpdate);

            }
            else
            {
                // update info (co the co password hoac khong)
                result = _bLSetting.UpdateInfo<UserCharityUpdate>(id, roleId, true, userCharityUpdate);

            }
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
