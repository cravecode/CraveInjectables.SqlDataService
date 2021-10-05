using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CraveInjectables.SqlDataService
{
    public interface ISqlDataService
    {
        bool ExecuteNonQuery(string sql, IList<SqlParameter> parameters, Func<int, bool> callback);

        bool ExecuteScalar(string sql, IList<SqlParameter> parameters, Func<object, bool> callback);

        bool ExecuteSqlReader(string sql, IList<SqlParameter> parameters, Func<IDataReader, bool> callback);
    }
}