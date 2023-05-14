using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth;
using Base;
using CharityBackendDL;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;

namespace Login
{
    public class DLLogin : DLBase, IDLLogin
    {
        public DLLogin(IDistributedCache distributedCache, IConfiguration configuration) : base(distributedCache, configuration)
        {
        }

        public int CreateUser(UserRegister user, List<string> excludeColumns)
        {
            return Insert(user, "user_account", excludeColumns);
            
        }

        public int DeleteCharity(int id)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "delete from charities where Id = @Id;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@Id", id);

                int _user = mySqlConnection.Execute(query, dynamicParameters);
                return _user;
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

        public int CreateCharity(InfoCharity infoCharity)
        {
            return InsertAndGetId(infoCharity, "charities");

        }

        public dynamic? GetUserByUsrNameOrId(string userName)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select * from charities c right join user_account ua on ua.CharityId = c.Id where UserName = @param limit 1;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@param", userName);

                dynamic _user = mySqlConnection.Query<dynamic>(query, dynamicParameters).FirstOrDefault();
                return _user ?? null;
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

        /// <summary>
        /// Luu token vao cache
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="Exception"></exception>
        public void SaveToken(int id, string token)
        {
            try
            {
                SaveDataRedis(id.ToString(), token, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetToken(int userId)
        {
            try
            {
                // Get the cached refreshToken value from Redis
                return GetDataRedis<string>(userId.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Logout(int userId)
        {
            DeleteDataRedis(userId.ToString());
        }
    }
}
