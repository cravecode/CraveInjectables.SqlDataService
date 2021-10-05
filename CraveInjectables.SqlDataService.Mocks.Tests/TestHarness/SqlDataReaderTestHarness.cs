using System;

namespace CraveInjectables.SqlDataService.Mocks.Tests
{
    public class SqlDataReaderTestHarness
    {
        #region Members

        private readonly ISqlDataService _SqlDataService;

        #endregion Members

        #region Constructors

        public SqlDataReaderTestHarness(ISqlDataService sqlDataService)
        {
            this._SqlDataService = sqlDataService;
        }

        #endregion Constructors

        #region Methods

        private static void ErrorIfNotExecptedValue<T>(object rdrValue, T expectedValue)
            where T : IComparable
        {
            if (expectedValue.CompareTo(rdrValue) != 0)
            {
                throw new ArgumentOutOfRangeException("");
            }
        }

        public void MockBusinessLogicUsingSqlDataReader()
        {
            var sql = "/*TestHarnessSqlService:SQL*/ Select FakeColumn1, FakeColumn2, FakeColumn3 FROM FakeTable;";
            _SqlDataService.ExecuteSqlReader(sql, null, (rdr) =>
            {
                int i = 0;
                while (rdr.Read())
                {
                    i++;

                    // Some pretend logic that is encapsulated in this function and would otherwise not be testable.
                    // We'll be testing the results of the IDataReader by the Mocked version and hijacking the results callback.
                    ErrorIfNotExecptedValue(rdr["FakeColumn1"], $"Row{i}Column1Value");
                    ErrorIfNotExecptedValue(rdr["FakeColumn2"], $"Row{i}Column2Value");
                    ErrorIfNotExecptedValue(rdr["FakeColumn3"], $"Row{i}Column3Value");
                }
                return true;
            });
        }

        #endregion Methods
    }
}