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
        public dynamic? GetUserByUsrNameOrId(string userName);

        public int CreateUser(UserRegister user, List<string> excludeColumns);

        public int DeleteCharity(int id);
        public int CreateCharity(InfoCharity infoCharity);

        public void SaveToken(int id, string token);

        public string GetToken(int userId);

        //public RefreshToken GetToken(string token);

        public int UpdateRefreshToken(Dictionary<string, string> columnUpdate, Dictionary<string, OperatorWhere> whereCondition);
    }
}
