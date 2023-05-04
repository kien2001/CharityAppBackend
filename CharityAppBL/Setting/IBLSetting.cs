using ActionResult;
using CharityAppBO.Setting;
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
        Task<ReturnResult> UpdateInfo(int id, bool isUpdatePassword, UserNormalUpdate userNormalUpdate);

        Task<ReturnResult> UpdateCharityInfo(int id, UserCharityUpdate userCharityUpdate);
        ReturnResult UpdateCharityPassword(int id, UpdatePassword updatePassword);

    }
}
