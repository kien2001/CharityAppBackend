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
using MySqlConnector;
using Newtonsoft.Json;

namespace Login
{
    public class DLLogin : DLBase, IDLLogin
    {
        private readonly IDistributedCache _redisCache;
        public DLLogin(IDistributedCache distributedCache)
        {
            _redisCache = distributedCache;
        } 

        public int CreateUser(UserRegister user)
        {
            return Insert(user, "user_account", new List<string>
            {
                "ConfirmPassword"
            });
            
        }

        public dynamic? GetUserByUsrNameOrId(string userName)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select * from user_account where UserName = @param limit 1;";

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
        /// <param name="refreshToken"></param>
        /// <exception cref="Exception"></exception>
        public void SaveToken(RefreshToken refreshToken)
        {
            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)); // Set expiration time to 1 day
                var refreshTokenJson = JsonConvert.SerializeObject(refreshToken);
                // Convert the JSON string to byte array
                var refreshTokenBytes = Encoding.UTF8.GetBytes(refreshTokenJson);
                _redisCache.Set(refreshToken.UserId.ToString(), refreshTokenBytes, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public RefreshToken? GetToken(int userId)
        {
            try
            {
                // Get the cached refreshToken value from Redis
                var refreshTokenBytes = _redisCache.Get(userId.ToString());

                if (refreshTokenBytes != null)
                {
                    // Deserialize the refreshToken object from the cached byte array
                    var refreshTokenJson = Encoding.UTF8.GetString(refreshTokenBytes);
                    var refreshToken = JsonConvert.DeserializeObject<RefreshToken>(refreshTokenJson);
                    return refreshToken;
                }

                return null; // Return null if refreshToken not found in cache
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int UpdateRefreshToken(Dictionary<string, string> columnUpdate, Dictionary<string, OperatorWhere> whereCondition)
        {
            return Update("refresh_token", columnUpdate, whereCondition);

        }
    }
}
