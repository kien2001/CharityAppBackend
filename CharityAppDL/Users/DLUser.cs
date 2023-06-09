﻿using CharityAppBO.Charity;
using CharityAppBO.Users;
using CharityBackendDL;
using Dapper;
using Login;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Users
{
    public class DLUser : IDLUser
    {
        public int ChangeStatusFollow(StatusFollow statusFollow)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = string.Empty;
                var dynamicParameters = new DynamicParameters();
                if(statusFollow.IsFollow)
                {
                    // follow
                    query = "insert charity_follow(userId, charityId) select @userId, @charityId WHERE NOT EXISTS (SELECT userId, charityId FROM charity_follow WHERE UserId = @userId AND CharityId = @charityId) ;";
                }
                else
                {
                    // unfollow
                    query = "delete from charity_follow where userId = @userId and charityId = @charityId;";
                }
                dynamicParameters.Add("userId", statusFollow.UserId);
                dynamicParameters.Add("charityId", statusFollow.CharityId);

                var result = mySqlConnection.Execute(query, dynamicParameters);
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

        public List<CharityFollow> GetFollowCharities(int userId)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "SELECT c.Id as CharityId,ua.Name AS CharityName,c.Avatar AS CharityImage,c.CharityDescription,c.IsVerified,c.CharityBanner FROM user_account ua  JOIN charities c  ON c.Id = ua.CharityId JOIN charity_follow cf  ON c.Id = cf.CharityId WHERE cf.UserId = @Id;";
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("Id", userId);

                var listCharities = mySqlConnection.Query<CharityFollow>(query, dynamicParameters).ToList();
                return listCharities;
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

        
        public List<Campaign> GetTopCampaign()
        {
            return null;
        }

        public List<TopCharity> GetTopCharity()
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "SET sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY',''));SELECT c.*, ua.Name AS CharityName, ua.PhoneNumber, ua.Address, ua.Email, t.NumberCampaign FROM (SELECT ci.organization_id, COUNT(ci.organization_id) AS NumberCampaign FROM campaign_info ci GROUP BY ci.organization_id ORDER BY NumberCampaign DESC LIMIT 3) AS t JOIN charities c ON c.Id = t.organization_id JOIN user_account ua ON ua.CharityId = c.Id;";
               
                var listCharities = mySqlConnection.Query<TopCharity>(query).ToList();
                return listCharities;
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
