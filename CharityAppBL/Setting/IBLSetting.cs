using ActionResult;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Setting
{
    public interface IBLSetting
    {
        ReturnResult CheckPassword(int id, string password);
        ReturnResult UpdateInfo(int id, int roleId, string isUpdatePassword, object userUpdate);

    }
}
