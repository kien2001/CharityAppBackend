using Auth;
using CharityAppBO;
using CharityBackendDL;
using Dapper;
using Firebase.Auth;
using Firebase.Storage;
using Login;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class DLBase : IDLBase
    {
        private readonly IDistributedCache _redisCache;
        private string ApiKey;
        private string Bucket;
        private string AuthEmail;
        private string AuthPassword;
       
        public DLBase(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _redisCache = distributedCache;
            ApiKey = configuration.GetSection("FirebaseSettings").GetValue<string>("ApiKey");
            Bucket = configuration.GetSection("FirebaseSettings").GetValue<string>("Bucket");
            AuthEmail = configuration.GetSection("FirebaseSettings").GetValue<string>("AuthEmail");
            AuthPassword = configuration.GetSection("FirebaseSettings").GetValue<string>("AuthPassword");

        }

        public T? GetDataRedis<T>(string key) where T : class
        {
            try
            {
                // Get the cached refreshToken value from Redis
                var dataBytes = _redisCache.Get(key);

                if (dataBytes != null)
                {
                    // Deserialize the refreshToken object from the cached byte array
                    var dataJson = Encoding.UTF8.GetString(dataBytes);
                    var data = JsonConvert.DeserializeObject<T>(dataJson);
                    return data;
                }

                return null; // Return null if refreshToken not found in cache
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Insert<T>(T entity, string tableName, List<string>? excludeColumns = null) where T : class
        {
            if(excludeColumns == null)
            {
                excludeColumns = new List<string>() { "Id"};
            }
            else
            {
                excludeColumns.Add("Id");
            }
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            using MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
            try
            {
                string query = $"Insert into {tableName}";
                var properties = entity.GetType().GetProperties().Where(p => excludeColumns.All(q => p.Name != q));
                string field = string.Join(", ", properties.Select(p => p.Name));
                string dynamicParam = string.Join(", ", properties.Select(p => $"@{p.Name}"));
                query = $"{query} ({field}) values ({dynamicParam});";
                DynamicParameters dynamicParameters = new();
                foreach (var prop in properties)
                {
                    dynamicParameters.Add($"@{prop.Name}", prop.GetValue(entity));
                }
                var result = mySqlConnection.Execute(query, dynamicParameters, mySqlTransaction);
                mySqlTransaction.Commit();
                return result;

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

        public int InsertAndGetId<T>(T entity, string tableName, List<string>? excludeColumns = null) where T : class
        {
            if (excludeColumns == null)
            {
                excludeColumns = new List<string>() { "Id" };
            }
            else
            {
                excludeColumns.Add("Id");
            }
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            using MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
            try
            {
                string query = $"Insert into {tableName}";
                var properties = entity.GetType().GetProperties().Where(p => excludeColumns.All(q => p.Name != q));
                string field = string.Join(", ", properties.Select(p => p.Name));
                string dynamicParam = string.Join(", ", properties.Select(p => $"@{p.Name}"));
                query = $"{query} ({field}) values ({dynamicParam});SELECT LAST_INSERT_ID()";
                DynamicParameters dynamicParameters = new();
                foreach (var prop in properties)
                {
                    dynamicParameters.Add($"@{prop.Name}", prop.GetValue(entity));
                }
                var result = mySqlConnection.ExecuteScalar<int>(query, dynamicParameters, mySqlTransaction);
                mySqlTransaction.Commit();
                return result;

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

        public void SaveDataRedis(string key, object data, DistributedCacheEntryOptions? distributedCacheEntryOptions)
        {
            try
            {
                if(distributedCacheEntryOptions == null)
                {
                    distributedCacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)); // Set expiration time to 1 day
                }
                var serializedData = JsonConvert.SerializeObject(data);
                // Convert the JSON string to byte array
                var dataBytes = Encoding.UTF8.GetBytes(serializedData);
                _redisCache.Set(key, dataBytes, distributedCacheEntryOptions);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int Update(string tableName, Dictionary<string, string> updateColumns, Dictionary<string, OperatorWhere> whereCondition)
        {
            using MySqlConnection mySqlConnection = new(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
            using MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
            try
            {
                string query = $"Update {tableName} set ";
                var columnUpdate = new List<string>();
                foreach (var column in updateColumns)
                {
                    string field = $"{column.Key} = @{column.Key}";
                    columnUpdate.Add(field);
                }
                string columnStr = string.Join(", ", columnUpdate);
                string whereConditionStr = BuidWhereQuery(whereCondition);

                if (!string.IsNullOrEmpty(whereConditionStr))
                {
                    columnStr += $" Where {whereConditionStr}";
                }

                query += columnStr;

                DynamicParameters dynamicParameters = new();
                foreach (var column in updateColumns)
                {
                    dynamicParameters.Add($"@{column.Key}", column.Value);
                }
                var result = mySqlConnection.Execute(query, dynamicParameters, mySqlTransaction);
                mySqlTransaction.Commit();
                return result;
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

        public async Task<string> UploadFileFirebase(MemoryStream memoryStream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("avatar")
                .Child(fileName)
                .PutAsync(memoryStream, cancellation.Token);

            string downloadUrl = "";
            try
            {
                downloadUrl = await task;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return downloadUrl;
        }

        private string BuidWhereQuery(Dictionary<string, OperatorWhere> whereCondition)
        {
            var whereQuery = new List<string>();
            foreach (var item in whereCondition)
            {
                string query = "";
                switch (item.Value.Operator)
                {
                    case Operator.Equal:
                        query = $"{item.Key} = {item.Value.Value}";
                        break;
                    case Operator.NotEqual:
                        query = $"{item.Key} <> {item.Value.Value}";
                        break;
                    case Operator.Like:
                        //query = $"{item.Key} like {item.Value.Value}";
                        break;
                    case Operator.In:
                        break;
                    case Operator.NotIn:
                        break;
                    default:
                        break;
                }
                whereQuery.Add(query);
            }
            return whereQuery.Count > 0 ? string.Join(" And ", whereQuery) : string.Empty;
        }
    }

    
}
