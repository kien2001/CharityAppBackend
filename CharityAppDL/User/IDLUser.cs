using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.User
{
    public interface IDLUser
    {
        dynamic GetUser(int id);

        List<dynamic> GetAllUser();

        int UpdateStatusUser(int id, bool status);

    }
}
