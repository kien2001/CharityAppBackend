using ActionResult;
using Auth;
using CharityAppBO.Login;
using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppBL.Login
{
    public interface IBLLogin
    {
        Task<ReturnResult> Authenthicate(UserLogin user);

        ReturnResult Register(UserRegister user);

        ReturnResult Logout(int id);

        Task<ReturnResult> ForgetPassword(string userName);
        ReturnResult ResetPassword(ResetPassword resetPassword);
        ReturnResult ResetCode(ResetCode resetCode);

    }
}
