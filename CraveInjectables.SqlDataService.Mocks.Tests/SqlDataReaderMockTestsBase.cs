using System.Collections.Generic;
using System.Data;
using Xunit;

namespace CraveInjectables.SqlDataService.Mocks.Tests
{
    public abstract class SqlDataReaderMockTestsBase
    {
        #region Members

        protected DataReaderMock<TestHarnessDataStructure> MockedReader { get; } = new DataReaderMock<TestHarnessDataStructure>(
            new List<TestHarnessDataStructure>() {
                new TestHarnessDataStructure { FakeColumn1 = "Row1Column1Value", FakeColumn2 = "Row1Column2Value", FakeColumn3 = "Row1Column3Value" },
                new TestHarnessDataStructure { FakeColumn1 = "Row2Column1Value", FakeColumn2 = "Row2Column2Value", FakeColumn3 = "Row2Column3Value" }
            });

        protected bool MocksExecuted { get; set; }

        #endregion Members



        #region Methods

        protected void BaseMockTest(ISqlDataService sqlService)
        {
            var testHarness = new SqlDataReaderTestHarness(sqlService);

            // Execute a fake scenario of a function that uses the ExecuteSqlReader in the ISqlDataService.
            testHarness.MockBusinessLogicUsingSqlDataReader();

            Assert.True(MocksExecuted);
        }

        protected void VerifyResults(IDataReader rdr)
        {
            // Here we verify that the callback was actually executed.
            // We could also verify some data i the Reader (rdr) is as expected, however our test harness's business logic throws an exception if it's not as expected.
            MocksExecuted = true;
        }

        #endregion Methods
    }
}