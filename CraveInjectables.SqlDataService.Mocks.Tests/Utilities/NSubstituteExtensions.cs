using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace CraveInjectables.SqlDataService.Mocks.Tests.Utilities
{
    public static class NSubstituteExtensions
    {
        public static void SetupDataReader<TData>(this ISqlDataService sqlService, Expression<Predicate<string>> matchPattern, Expression<Predicate<IList<SqlParameter>>> paramsMatch, DataReaderMock<TData> dataReaderMock, Action<DataReaderMock<TData>> callback)
        {
            sqlService.ExecuteSqlReader(
                Arg.Is(matchPattern),
                Arg.Is(paramsMatch),
                Arg.Do<Func<IDataReader, bool>>(x =>
                {
                    var result = x.Invoke(dataReaderMock);
                    dataReaderMock.Enumerator.Reset();
                    callback(dataReaderMock);
                }));
        }
    }
}