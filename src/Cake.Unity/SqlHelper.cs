using System.Data.OleDb;

namespace Cake.Mix
{
    internal sealed class SqlHelper
    {
        private readonly OleDbConnection _connection;
        private readonly OleDbTransaction _transaction;

        internal SqlHelper(string connectionString, bool useTransaction)
        {
            _connection = new OleDbConnection(connectionString);
            _connection.Open();

            if (useTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        internal void Close(bool commitTransaction = true)
        {
            if (_transaction != null)
            {
                if (commitTransaction)
                {
                    _transaction.Commit();
                }
                else
                {
                    _transaction.Rollback();
                }
            }

            _connection.Close();
        }

        internal void Execute(string sql)
        {
            var command = new OleDbCommand(sql, _connection);
            if (_transaction != null) { command.Transaction = _transaction; }
            command.ExecuteReader();   
        }
    }
}
