using Base;
using CharityAppBO.Charity;
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

namespace CharityAppDL.Charity
{
    public class DLCharity : DLBase, IDLCharity
    {
        public DLCharity(IDistributedCache distributedCache, IConfiguration configuration) : base(distributedCache, configuration)
        {
        }

        public List<CharityItem> GetAllCharity()
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            try
            {
                string query = "Select ua.Id, ua.Name from user_account ua join charities c on ua.CharityId = c.Id where ua.IsLocked is false;";

                var listCharities = mySqlConnection.Query<CharityItem>(query).ToList();
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
