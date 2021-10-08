using Crave.Injectable.SqlData.Mocks.Tests.TestHarness;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace CraveInjectables.SqlDataService.Mocks.Tests
{
    public class SqlDataReaderMockNSubstituteTests
    {
        [Fact]
        public void MockedReaderValuesTest()
        {
            IDataReader mockedReader = new DataReaderMock<object>(
                new List<object>() {
                new { Id = 1, FirstName = "Foo", LastName = "Bar", Status = 3 },
                new { Id = 1, FirstName = "Fiz", LastName = "Baz", Status = 3 },
                });

            var sqlService = Substitute.For<ISqlDataService>();

            sqlService.ExecuteSqlReader(
                Arg.Is<string>(x => x.Contains("SampleService:SampleMethod")),
                Arg.Any<IList<SqlParameter>>(),
                Arg.Do<Action<IDataReader>>(x =>
                {
                    x.Invoke(mockedReader);
                }));

            var errorMsg = string.Empty;

            try
            {
                new SampleService(sqlService).SampleMethod();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            Assert.Contains("more than 1", errorMsg);
        }
    }
}