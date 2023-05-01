using CharityBackendDL;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.User
{
    public class DLUser : IDLUser
    {
        public List<dynamic> GetAllUser()
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select ua.* from user_account ua join role r on ua.RoleId = r.Id where r.RoleName <> 'Admin';";

                var listUser = mySqlConnection.Query(query).ToList();
                return listUser;
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

        public dynamic GetUser(int id)
        {

            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select * from user_account where Id = @param limit 1;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@param", id);

                dynamic _user = mySqlConnection.Query<dynamic>(query, dynamicParameters).FirstOrDefault();
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

        public int UpdateStatusUser(int id, bool status)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "update user_account set IsLocked = @IsLocked where Id = @Id;";

                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("@IsLocked", status);
                dynamicParameters.Add("@Id", id);

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
