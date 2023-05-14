using ActionResult;
using Auth;
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

    }
}
