using Crave.Injectable.SqlData.Mocks.Tests.TestHarness;
using CraveInjectables.SqlDataService;
using CraveInjectables.SqlDataService.Mocks;
using CraveInjectables.SqlDataService.Mocks.Tests;
using CraveInjectables.SqlDataService.Mocks.Tests.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Crave.Injectable.SqlData.Mocks.Tests
{
    public class SqlDataReaderMockMoqTests
    {
        [Fact]
        public void MockedReaderValuesTest()
        {
            IDataReader mockedReader = new DataReaderMock<object>(
                new List<object>() {
                new { Id = 1, FirstName = "Foo", LastName = "Bar", Status = 3 },
                new { Id = 1, FirstName = "Fiz", LastName = "Baz", Status = 3 },
                });

            Mock<ISqlDataService> sqlService = new Mock<ISqlDataService>();
            sqlService.Setup(
                x => x.ExecuteSqlReader(
                    It.Is<string>(x => x.Contains("SampleService:SampleMethod")),
                    It.IsAny<IList<SqlParameter>>(),
                    It.IsAny<Action<IDataReader>>())
                ).Callback((string sql, IList<SqlParameter> parameters, Action<IDataReader> sourceCallback) =>
                {
                    // Call the underlying code's provided callback first so that the whole function can execute.
                    sourceCallback(mockedReader);
                });

            var errorMsg = string.Empty;

            try
            {
                new SampleService(sqlService.Object).SampleMethod();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;

            }
            Assert.Contains("more than 1", errorMsg);
        }
    }
}