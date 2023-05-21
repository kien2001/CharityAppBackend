using ActionResult;
using CharityAppBO.Users;
using CharityAppDL.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Users
{
    public class BLUser : IBLUser
    {
        private IDLUser _dlUser;
        public BLUser(IDLUser iDLUser)
        {
            this._dlUser = iDLUser;
        }

        public ReturnResult ChangeStatusFollow(StatusFollow statusFollow)
        {
            var result = new ReturnResult();
            try
            {
                var _rs = _dlUser.ChangeStatusFollow(statusFollow);
                if(_rs > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string> { "Bạn đã bỏ theo dõi tổ chức này" });
                }
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

        public ReturnResult GetFollowCharities(int userId)
        {
            var result = new ReturnResult();
            try
            {
                var listCharities = _dlUser.GetFollowCharities(userId);
                result.Ok(listCharities);
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

        public ReturnResult GetTopCampaign()
        {
            var result = new ReturnResult();
            try
            {
                var listCharities = _dlUser.GetTopCampaign();
                result.Ok(listCharities);
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

    }
}
