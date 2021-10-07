using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CraveInjectables.SqlDataService
{
    public interface ISqlDataService
    {
        void ExecuteNonQuery(string sql, IList<SqlParameter> parameters, Action<int> callback);

        void ExecuteScalar(string sql, IList<SqlParameter> parameters, Action<object> callback);

        void ExecuteSqlReader(string sql, IList<SqlParameter> parameters, Action<IDataReader> callback);
    }
}