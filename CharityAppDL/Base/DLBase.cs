using CharityAppBO;
using CharityBackendDL;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class DLBase : IDLBase
    {
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
