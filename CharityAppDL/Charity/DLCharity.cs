﻿using Base;
using CharityAppBO.Charity;
using CharityAppBO.Users;
using CharityBackendDL;
using Dapper;
using Login;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityAppDL.Charity
{
    public class DLCharity : DLBase, IDLCharity
    {
        public DLCharity(IDistributedCache distributedCache, IConfiguration configuration) : base(distributedCache, configuration)
        {
        }


        public List<CharityFollow> GetAllCharities(int? userId)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = string.Empty;
                DynamicParameters dynamicParameters = new DynamicParameters();
                if(userId == null)
                {
                    query = "SELECT c.Id as CharityId,ua.Name AS CharityName, false as IsFollow, c.Avatar AS CharityImage,c.CharityDescription,c.IsVerified,c.CharityBanner FROM user_account ua  JOIN charities c  ON c.Id = ua.CharityId;";
                }
                else
                {
                    query = "SET  sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY',''));SELECT * from (SELECT  c.Id as CharityId,ua.Name AS CharityName,c.Avatar AS CharityImage, true AS IsFollow, c.CharityDescription,c.IsVerified,c.CharityBanner FROM user_account ua JOIN charities c  ON c.Id = ua.CharityId JOIN charity_follow cf  ON c.Id = cf.CharityId WHERE cf.UserId = @userId UNION SELECT c.Id as CharityId,ua.Name AS CharityName,c.Avatar AS CharityImage, FALSE AS IsFollow,c.CharityDescription,c.IsVerified,c.CharityBanner FROM charities c JOIN user_account ua  ON c.Id = ua.CharityId GROUP BY c.Id, ua.Name) t GROUP BY t.CharityId; ";
                    dynamicParameters.Add("@userId", userId);
                }
               
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

        public CharityObj GetCharityById(int charityId, int? userId)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = string.Empty;
                var dynamicParam = new DynamicParameters();
                if (userId == null)
                {
                    query = "Select ua.Name as CharityName, c.* from user_account ua join charities c on ua.CharityId = c.Id where ua.CharityId = @charityId  limit 1;";
                    dynamicParam.Add("@charityId", charityId);
                }
                else
                {
                    query = "Select ua.Name as CharityName, c.*, IF(@userId = (SELECT userId FROM charity_follow cf WHERE cf.UserId = @userId AND cf.CharityId = @charityId), true, false) AS IsFollow from user_account ua join charities c on ua.CharityId = c.Id where ua.CharityId = @charityId  limit 1;";
                    dynamicParam.Add("@charityId", charityId);
                    dynamicParam.Add("@userId", userId);
                }
                var charity = mySqlConnection.QueryFirstOrDefault<CharityObj>(query, dynamicParam);
                return charity;
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