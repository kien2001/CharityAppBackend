using Base;
using CharityBackendDL;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Setting
{
    public class DLSetting : DLBase, IDLSetting
    {
        public DLSetting(IDistributedCache distributedCache, IConfiguration configuration) : base(distributedCache, configuration)
        {
        }

        public dynamic GetPasswordById(int id)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select Password, SaltPassword from user_account where Id = @Id limit 1;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@Id", id);

                dynamic _user = mySqlConnection.QueryFirstOrDefault(query, dynamicParameters);
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

        public int UpdateInfo(string tableName, Dictionary<string, string> updateColumns, Dictionary<string, OperatorWhere> whereCondition)
        {
            return Update(tableName, updateColumns, whereCondition);
        }

        
        public int UpdateCharityPassword(int id, string newPassword)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "update user_account set Password = @Password where Id = @Id;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@Id", id);
                dynamicParameters.Add("@Password", newPassword);

                int result = mySqlConnection.Execute(query, dynamicParameters);
                return result;
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
}
