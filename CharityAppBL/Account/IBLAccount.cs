using ActionResult;
using CharityAppBO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Users
{
    public interface IBLAccount
    {
        ReturnResult GetUser(int id);

        ReturnResult GetAllUser();

        ReturnResult ChangeStatus(UpdateStatusUser updateStatusUser);


    }
}
