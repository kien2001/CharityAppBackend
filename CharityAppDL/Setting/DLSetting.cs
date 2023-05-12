using Base;
using CharityAppBO.Setting;
using CharityBackendDL;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public int UpdateCharityInfo(int id, UserCharityUpdate userCharityUpdate)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            using MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
            try
            {
                // Update thong tin to chuc
                DynamicParameters dynamicParameters = new();
                string queryCharity = GenerateQuery(userCharityUpdate.CharityId, "charities", userCharityUpdate.CharityInfo, ref dynamicParameters);
                var result = mySqlConnection.Execute(queryCharity, dynamicParameters, mySqlTransaction);
                
                
                // update thong tin tai khoan
                var userAccount = CharityUtil.ConvertToType<UserNormalUpdate>(userCharityUpdate);
                DynamicParameters dynamicParameter1 = new();

                string queryUser = GenerateQuery(id, "user_account", userAccount, ref dynamicParameter1);
                var result1 = mySqlConnection.Execute(queryUser, dynamicParameter1, mySqlTransaction);
                if (result1 == 0)
                {
                    mySqlTransaction.Rollback();
                }
                else
                {
                    mySqlTransaction.Commit();
                }

                return result1;
            }
            catch (MySqlException ex)
            {
                mySqlTransaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private string GenerateQuery(int idUpdate, string table, object objUpdate, ref DynamicParameters dynamicParameters)
        {
            string firstQuery = $"Update {table} set ";
            var columnUpdate = new List<string>();
            foreach (var property in objUpdate.GetType().GetProperties())
            {
                string field = $"{property.Name} = @{property.Name}";
                columnUpdate.Add(field);
            }
            string columnStr = string.Join(", ", columnUpdate);
            columnStr += $" Where Id = {idUpdate}";

            firstQuery += columnStr;
            foreach (var column in objUpdate.GetType().GetProperties())
            {
                dynamicParameters.Add($"@{column.Name}", column.GetValue(objUpdate));
            }
            return firstQuery;
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
