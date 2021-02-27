using DatabaseAdapter.Contract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace DatabaseAdapter.Service
{
    public abstract class BaseDastabaseAdapter : IDatabaseAdapter, IDisposable, IAsyncDisposable
    {
        protected DbConnection DbConnection { get; private set; }
        public BaseDastabaseAdapter(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }
        public void Dispose()
        {
            if (DbConnection.State != ConnectionState.Closed)
                DbConnection.Close();
            DbConnection = null;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (DbConnection.State != ConnectionState.Closed)
                await CloseConnectionAsync();
            DbConnection = null;
            GC.SuppressFinalize(this);
        }        
        public const int DbTimeout = 30;        
        public int CommandTimeout { get; set; } = DbTimeout;        

        private async Task OpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (DbConnection?.State != System.Data.ConnectionState.Open)
                await DbConnection.OpenAsync(cancellationToken);
        }

        private async Task CloseConnectionAsync()
        {
            if (DbConnection?.State != System.Data.ConnectionState.Closed)
                await DbConnection.CloseAsync();
        }

        private CancellationToken GetToken()
        {
            var tokenSource = new CancellationTokenSource(DbTimeout * 1000);
            var cancellationToken = tokenSource.Token;
            return cancellationToken;
        }
        
        public virtual DbParameter CreateDbParameter(string parameterName, DbType dbType, object value, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            var dbParameter = DbConnection.CreateCommand().CreateParameter();
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = value;
            dbParameter.DbType = dbType;
            if (parameterDirection != ParameterDirection.Input)
                dbParameter.Direction = parameterDirection;
            return dbParameter;
        }

        public abstract DbParameter CreateDbParameter(string typeName, string parameterName, object value);


        public virtual DbCommand CreateCommand(string commandText, DbParameter[] parameters, CommandType commandType)
        {
            DbCommand dbCommand = DbConnection.CreateCommand();
            dbCommand.CommandText = commandText;
            if (parameters?.Any() ?? false)
            {
                dbCommand.Parameters.AddRange(parameters);
            }
            dbCommand.CommandType = commandType;
            if (CommandTimeout >= 0 && CommandTimeout != DbTimeout)
                dbCommand.CommandTimeout = CommandTimeout;
            return dbCommand;
        }

        public virtual DbParameter[] CreateDbParameterCollection(Tuple<string, DbType, object>[] parameters)
        {
            return parameters.Select(parameter => CreateDbParameter(parameter.Item1, parameter.Item2, parameter.Item3)).ToArray();
        }

        public async IAsyncEnumerable<IDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                command.Connection = DbConnection;
                cancellationToken.Value.Register(() => command.Cancel());
                using (var dataReader = await command.ExecuteReaderAsync(cancellationToken.Value))
                {
                    while (await dataReader.ReadAsync())
                    {
                        yield return dataReader;
                    }
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async IAsyncEnumerable<IDataReader> ExecuteReaderAsync(string commandText, DbParameter[] dbParameters = null, 
            CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                var dbcommand = CreateCommand(commandText, dbParameters, commandType);
                cancellationToken.Value.Register(() => dbcommand.Cancel());
                using (var dataReader = await dbcommand.ExecuteReaderAsync(cancellationToken.Value))
                {
                    while (await dataReader.ReadAsync())
                    {
                        yield return dataReader;
                    }
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }       

        public async Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                command.Connection = DbConnection;
                cancellationToken.Value.Register(() => command.Cancel());
                var scalarValue = await command.ExecuteScalarAsync(cancellationToken.Value);
                return scalarValue;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<object> ExecuteScalarAsync(string commandText, DbParameter[] dbParameters = null, 
            CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                var dbcommand = CreateCommand(commandText, dbParameters, commandType);
                cancellationToken.Value.Register(() => dbcommand.Cancel());
                var scalarValue = await dbcommand.ExecuteScalarAsync(cancellationToken.Value);
                return scalarValue;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                command.Connection = DbConnection;
                cancellationToken.Value.Register(() => command.Cancel());
                var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken.Value);
                return rowsAffected;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string commandText, DbParameter[] dbParameters = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken? cancellationToken = null)
        {
            try
            {
                cancellationToken ??= GetToken();
                await OpenConnectionAsync(cancellationToken.Value);
                var dbcommand = CreateCommand(commandText, dbParameters, commandType);
                cancellationToken.Value.Register(() => dbcommand.Cancel());
                var rowsAffected = await dbcommand.ExecuteNonQueryAsync(cancellationToken.Value);
                return rowsAffected;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(DbConnection);
            DataSet dataSet = new DataSet();
            DbDataAdapter adapter = factory.CreateDataAdapter();
            command.Connection = DbConnection;
            adapter.SelectCommand = command;
            adapter.Fill(dataSet);
            return dataSet;
        }

        public virtual DataSet ExecuteDataSet(string commandText, DbParameter[] dbParameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            var dbcommand = CreateCommand(commandText, dbParameters, commandType);
            DbProviderFactory factory = DbProviderFactories.GetFactory(DbConnection);
            DataSet dataSet = new DataSet();
            DbDataAdapter adapter = factory.CreateDataAdapter();
            dbcommand.Connection = DbConnection;
            adapter.SelectCommand = dbcommand;
            adapter.Fill(dataSet);
            return dataSet;
        }
    }
}
