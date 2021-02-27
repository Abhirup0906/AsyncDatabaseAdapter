using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseAdapter.Contract
{
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Specify command time out to override default command time out. Default value is int.MinValue
        /// </summary>
        int CommandTimeout { get; set; }
        /// <summary>
        /// Create Database command 
        /// </summary>
        /// <param name="commandText">SP name with schema or sql query</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="commandType">Type of command SP/Text</param>
        /// <returns></returns>
        DbCommand CreateCommand(string commandText, DbParameter[] parameters, CommandType commandType);
        /// <summary>
        /// Create DbParameter which can be passed with SP/Sql query
        /// </summary>
        /// <param name="parameterName">Name of the SQL parameter</param>
        /// <param name="dbType">Database type</param>
        /// <param name="value">Parameter value</param>
        /// <param name="parameterDirection">Parameter direction. Default is input</param>
        /// <returns></returns>
        DbParameter CreateDbParameter(string parameterName, DbType dbType, object value, ParameterDirection parameterDirection = ParameterDirection.Input);
        /// <summary>
        /// Create DbParameter[] which can be passed with SP/Sql query
        /// </summary>
        /// <param name="parameters">List of parameters (ParameterName, DbType, Value)</param>
        /// <returns></returns>
        DbParameter[] CreateDbParameterCollection(Tuple<string, DbType, object>[] parameters);
        /// <summary>
        /// Create DbParameter for custom sql types
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateDbParameter(string typeName, string parameterName, object value);
        /// <summary>
        /// Return result set 
        /// </summary>
        /// <param name="command">Database command</param>
        /// <returns></returns>
        IAsyncEnumerable<IDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken? cancellationToken = null);
        /// <summary>
        /// Return result set 
        /// </summary>
        /// <param name="commandText">SP name with schema or sql query</param>
        /// <param name="dbParameters">List of parameters</param>
        /// <param name="commandType">Type of command SP/Text</param>
        /// <returns></returns>
        IAsyncEnumerable<IDataReader> ExecuteReaderAsync(string commandText, DbParameter[] dbParameters = null, 
            CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null);                
        /// <summary>
        /// Return single result 
        /// </summary>
        /// <param name="command">Database command</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken? cancellationToken = null);
        /// <summary>
        /// Return single result
        /// </summary>
        /// <param name="commandText">SP name with schema or sql query</param>
        /// <param name="dbParameters">List of parameters</param>
        /// <param name="commandType">Type of command SP/Text</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandText, DbParameter[] dbParameters = null, 
            CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null);        
        /// <summary>
        /// Retuns number of rows affected
        /// </summary>
        /// <param name="command">Database command</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken? cancellationToken = null);
        /// <summary>
        /// Retuns number of rows affected
        /// </summary>
        /// <param name="commandText">SP name with schema or sql query</param>
        /// <param name="dbParameters">List of parameters</param>
        /// <param name="commandType">Type of command SP/Text</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandText, DbParameter[] dbParameters = null, 
            CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null);        
        /// <summary>
        /// Return dataset 
        /// </summary>
        /// <param name="command">Database command</param>
        /// <returns></returns>
        DataSet ExecuteDataSet(DbCommand command);
        /// <summary>
        /// Return dataset
        /// </summary>
        /// <param name="commandText">SP name with schema or sql query</param>
        /// <param name="dbParameters">List of parameters</param>
        /// <param name="commandType">Type of command SP/Text</param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string commandText, DbParameter[] dbParameters = null, CommandType commandType = CommandType.StoredProcedure);
    }
}
