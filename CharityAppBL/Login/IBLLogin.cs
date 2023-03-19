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
        public ReturnResult Authenthicate(UserLogin user);

        public ReturnResult Register(UserRegister user);

        public ReturnResult VerifyToken(TokenRequest tokenRequest);

    }
}
