using Auth;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    public interface IDLLogin
    {
        public User GetUserByUsrNameOrId(object param);

        public int CreateUser(UserRegister user);

        public int SaveToken(RefreshToken refreshToken);

        public RefreshToken GetToken(string token);

        public int UpdateRefreshToken(Dictionary<string, string> columnUpdate, Dictionary<string, OperatorWhere> whereCondition);
    }
}
