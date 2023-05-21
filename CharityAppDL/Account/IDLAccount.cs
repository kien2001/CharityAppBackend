using CharityAppBO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.User
{
    public interface IDLAccount
    {
        (dynamic?, int, int) GetUser(int id);

        List<dynamic> GetAllUser();

        int UpdateStatusUser(int id, bool status);

        int ChangeStatusVerify(int charityId, bool isAccepted, string? message);

        List<VerifyCharity> GetAllStatusVerify();

    }
}
