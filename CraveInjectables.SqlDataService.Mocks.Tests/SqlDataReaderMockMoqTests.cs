using CraveInjectables.SqlDataService;
using CraveInjectables.SqlDataService.Mocks.Tests;
using CraveInjectables.SqlDataService.Mocks.Tests.Utilities;
using Moq;
using Xunit;

namespace Crave.Injectable.SqlData.Mocks.Tests
{
    public class SqlDataReaderMockMoqTests : SqlDataReaderMockTestsBase
    {
        [Fact]
        public void MockedReaderValuesTest()
        {
            Mock<ISqlDataService> sqlService = new Mock<ISqlDataService>();
            sqlService.SetupDataReader(
                // Only run this particular mock for specific SQL.
                sql => sql.Contains("TestHarnessSqlService:SQL"),
                x => true,
                MockedReader,
                VerifyResults);

            BaseMockTest(sqlService.Object);

            Assert.True(MocksExecuted);
        }
    }
}