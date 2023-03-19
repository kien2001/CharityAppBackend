using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth;
using Base;
using CharityBackendDL;
using Dapper;
using MySqlConnector;

namespace Login
{
    public class DLLogin : DLBase, IDLLogin
    {
        public int CreateUser(UserRegister user)
        {
            return Insert(user, "user_account", new List<string>
            {
                "ConfirmPassword"
            });
            
        }

        public RefreshToken GetToken(string token)
        {

            using (MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString))
            {

                mySqlConnection.Open();
                try
                {

                    string query = "Select * from refresh_token where token = @token limit 1;";
                    DynamicParameters dynamicParameters = new();
                    dynamicParameters.Add("@token", token);

                    var _token = mySqlConnection.QueryFirstOrDefault<RefreshToken>(query, dynamicParameters);
                    return _token;

                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
                
        }

        public User GetUserByUsrNameOrId(object param)
        {
            using (MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                try
                {
                    string query = "Select * from user_account where {0} = @param limit 1;";

                    if (param is string)
                    {
                        query = string.Format(query, "UserName");
                    }else if(param is int)
                    {
                        query = string.Format(query, "Id");
                    }

                    DynamicParameters dynamicParameters = new();
                    dynamicParameters.Add("@param", param);

                    User userLogin = mySqlConnection.QueryFirstOrDefault<User>(query, dynamicParameters);
                    return userLogin;
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
        }

        public User GetUserByUserName(int id)
        {
            throw new NotImplementedException();
        }

        public int SaveToken(RefreshToken refreshToken)
        {
            return Insert(refreshToken, "refresh_token");
          
        }

        public int UpdateRefreshToken(Dictionary<string, string> columnUpdate, Dictionary<string, OperatorWhere> whereCondition)
        {
            return Update("refresh_token", columnUpdate, whereCondition);

        }
    }
}
