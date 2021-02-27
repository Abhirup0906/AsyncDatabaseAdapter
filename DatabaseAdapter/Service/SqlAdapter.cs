using System.Data.Common;
using System.Data.SqlClient;

namespace DatabaseAdapter.Service
{
    public class SqlAdapter: BaseDastabaseAdapter
    {
        public SqlAdapter(SqlConnection sqlConnection) : base(sqlConnection)
        {

        }

        public override DbParameter CreateDbParameter(string typeName, string parameterName, object value)
        {
            var dbParameter = (new SqlCommand()).CreateParameter();
            dbParameter.ParameterName = typeName;
            dbParameter.Value = value;
            dbParameter.TypeName = typeName;
            return dbParameter;
        }
    }
}
