using CharityBackendDL;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.User
{
    public class DLAccount : IDLAccount
    {
        public List<dynamic> GetAllUser()
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select c.*, ua.* from charities c right join user_account ua on c.Id = ua.CharityId join role r on ua.RoleId = r.Id where r.RoleName <> 'Admin';";

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

        public (dynamic?, int, int) GetUser(int id)
        {

            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string proc = "Proc_GetCurrentUser";


                var results = mySqlConnection.QueryMultiple(proc, new { v_Id = id }, commandType: CommandType.StoredProcedure);

                var _user = results.Read<dynamic>().FirstOrDefault();
               
                int numFollow = results.Read<int>().FirstOrDefault();
                int numCampaign = results.Read<int>().FirstOrDefault();
                  
                return ( _user, numFollow, numCampaign);
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

        public int ChangeStatusVerify(int charityId, bool isAccepted, string? message)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = string.Empty;
                DynamicParameters dynamicParameters = new();

                if (isAccepted)
                {
                    // cho phep => thay doi isverified, xoa row trong bang verify
                    query = "delete from charity_process_verify where CharityId = @charityId;update charities set IsVerified = 2 where Id = @charityId";
                    dynamicParameters.Add("@charityId", charityId);
                }
                else
                {
                    // tu choi => update message vao bang verify, update isverified = 0, xoa charityFile
                    query = "update charity_process_verify set MessageToCharity = @message where CharityId = @charityId;update charities set IsVerified = 0, CharityFile = NULL where Id = @charityId";
                    dynamicParameters.Add("@charityId", charityId);
                    dynamicParameters.Add("@message", message);
                }

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
