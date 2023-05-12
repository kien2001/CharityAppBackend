using CharityAppBO.Users;
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
    }
}
