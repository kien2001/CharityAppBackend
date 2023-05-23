using CharityAppBO.Users;
using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Users
{
    public interface IDLUser
    {
        List<CharityFollow> GetFollowCharities(int userId);

        int ChangeStatusFollow(StatusFollow statusFollow);

        List<Campaign> GetTopCampaign();

        List<TopCharity> GetTopCharity();
    }
}
