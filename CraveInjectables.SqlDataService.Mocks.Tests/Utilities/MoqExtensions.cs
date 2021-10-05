using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace CraveInjectables.SqlDataService.Mocks.Tests.Utilities
{
    public static class MoqExtensions
    {
        public static void SetupDataReader<TData>(this Mock<ISqlDataService> moq, Expression<Func<string, bool>> matchPattern, Expression<Func<IList<SqlParameter>, bool>> paramsMatch, DataReaderMock<TData> dataReaderMock, Action<DataReaderMock<TData>> callback)
        {
            moq.Setup(
                x => x.ExecuteSqlReader(
                    It.Is(matchPattern),
                    It.Is(paramsMatch),
                    It.IsAny<Func<IDataReader, bool>>())
                ).Returns((string sql, IList<SqlParameter> parameters, Func<IDataReader, bool> sourceCallback) =>
                {
                    // Call the underlying code's provided callback first so that the whole function can execute.
                    var results = sourceCallback(dataReaderMock);

                    // Then call our evaluation on the reader to verify the data returned.
                    dataReaderMock.Enumerator.Reset();
                    callback(dataReaderMock);

                    return results;
                });
        }
    }
}