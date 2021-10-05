using CraveInjectables.SqlDataService.Mocks.Tests.Utilities;
using NSubstitute;
using Xunit;

namespace CraveInjectables.SqlDataService.Mocks.Tests
{
    public class SqlDataReaderMockNSubstituteTests : SqlDataReaderMockTestsBase
    {
        [Fact]
        public void MockedReaderValuesTest()
        {
            var sqlService = Substitute.For<ISqlDataService>();

            sqlService.SetupDataReader(
                x => x.Contains("TestHarnessSqlService:SQL"),
                x => true,
                MockedReader,
                VerifyResults);

            BaseMockTest(sqlService);

            Assert.True(MocksExecuted);
        }
    }
}