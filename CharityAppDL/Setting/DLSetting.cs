using CharityBackendDL;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Setting
{
    public class DLSetting : IDLSetting
    {
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
    }
}
