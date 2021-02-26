using DatabaseAdapter.Contract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseAdapter.Service
{
    public class BaseDastabaseAdapter : IDatabaseAdapter, IDisposable, IAsyncDisposable
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
                await DbConnection.CloseAsync();
            DbConnection = null;
            GC.SuppressFinalize(this);
        }
    }
}
