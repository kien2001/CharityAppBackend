using ActionResult;
using CharityAppBO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Users
{
    public interface IBLUser
    {
        ReturnResult GetFollowCharities(int userId);

        ReturnResult ChangeStatusFollow(StatusFollow statusFollow);

        ReturnResult GetTopCampaign();
    }
}
