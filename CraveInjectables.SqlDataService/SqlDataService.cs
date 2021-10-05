using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace CraveInjectables.SqlDataService
{
    public class SqlDataService : ISqlDataService
    {
        #region Members

        private readonly Func<SqlConnection> _CreateConnection;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Will create a SqlConnection using the provided connection string when attempting to execute the SqlCommand.
        /// </summary>
        /// <param name="sqlConnection"></param>
        public SqlDataService(string sqlConnection)
            : this(() => CreateConnectionFromString(sqlConnection))
        {
        }

        /// <summary>
        /// Will create the SqlConnection using the callback provided when attempting to Execute the Reader.
        /// </summary>
        /// <param name="createConnection"></param>
        public SqlDataService(Func<SqlConnection> createConnection)
        {
            _CreateConnection = createConnection;
        }

        #endregion Constructors

        #region Methods

        private static SqlConnection CreateConnectionFromString(string sqlConnectionString)
        {
            return new SqlConnection(sqlConnectionString);
        }

        protected bool ExecuteSqlCommand<TResult>(string sql, IList<SqlParameter> parameters, Expression<Func<SqlCommand, TResult>> expression, Func<TResult, bool> action)
        {
            using (var conn = _CreateConnection())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    IDisposable disposable = null;

                    if (null != parameters)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    // We have to write our own "using" statement out of a try/finally because we're executing from an expression tree.
                    try
                    {
                        // Here's where the magic happens. We'll execute either nonquery, sqldatareader, scalar. All based on the calling expression.
                        var invokedResult = expression.Compile().Invoke(cmd);

                        // If not IDisposable, will be null.
                        disposable = invokedResult as IDisposable;

                        return (action?.Invoke(invokedResult))
                            .GetValueOrDefault();
                    }
                    finally
                    {
                        disposable?.Dispose();
                    }
                }
            }
        }

        public bool ExecuteNonQuery(string sql, IList<SqlParameter> parameters, Func<int, bool> callback)
        {
            return ExecuteSqlCommand(
                sql,
                parameters,
                cmd => cmd.ExecuteNonQuery(),
                callback);
        }

        public bool ExecuteScalar(string sql, IList<SqlParameter> parameters, Func<object, bool> callback)
        {
            return ExecuteSqlCommand(
                sql,
                parameters,
                cmd => cmd.ExecuteScalar(),
                callback);
        }

        public bool ExecuteSqlReader(string sql, IList<SqlParameter> parameters, Func<IDataReader, bool> callback)
        {
            return ExecuteSqlCommand(
                sql,
                parameters,
                cmd => cmd.ExecuteReader(),
                callback);
        }

        #endregion Methods
    }
}