using CharityAppBO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public partial class OperatorWhere
    {
        public Operator Operator { get; set; }

        public string Value { get; set; }
    }

    public interface IDLBase
    {
        int Insert<T>(T entity, string tableName, List<string> excludeColumns) where T : class;

        int InsertAndGetId<T>(T entity, string tableName, List<string> excludeColumns) where T : class;


        int Update(string tableName, Dictionary<string, string> updateColumns, Dictionary<string, OperatorWhere> whereCondition);

        void SaveDataRedis(string key, object data, DistributedCacheEntryOptions? distributedCacheEntryOptions);

        T? GetDataRedis<T>(string key) where T : class;

        Task<string> UploadFileFirebase(IFormFile file, string fileName);

        void DeleteDataRedis(string redisKey);
    }
}
