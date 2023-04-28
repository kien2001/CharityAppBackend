using CharityAppBO;
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
        public int Insert<T>(T entity, string tableName, List<string> excludeColumns) where T : BaseEntity;

        public int Update(string tableName, Dictionary<string, string> updateColumns, Dictionary<string, OperatorWhere> whereCondition);

    }
}
