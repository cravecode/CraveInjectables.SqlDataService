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
                    It.IsAny<Action<IDataReader>>())
                ).Callback((string sql, IList<SqlParameter> parameters, Action<IDataReader> sourceCallback) =>
                {
                    // Call the underlying code's provided callback first so that the whole function can execute.
                    sourceCallback(dataReaderMock);

                    // Then call our evaluation on the reader to verify the data returned.
                    dataReaderMock.Enumerator.Reset();
                    callback(dataReaderMock);
                });
        }
    }
}