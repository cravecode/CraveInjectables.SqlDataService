using CraveInjectables.SqlDataService;
using System;
using System.Data.SqlClient;

namespace Crave.Injectable.SqlData.Mocks.Tests.TestHarness
{
    public class SampleService
    {

        private ISqlDataService _SqlDataService;

        public SampleService(ISqlDataService dataService)
        {
            _SqlDataService = dataService;
        }

        public void SampleMethod()
        {
            // Refactored previous code to use our SqlDataService as a wrapper to the SqlCommand and SqlDataReader.
            _SqlDataService.ExecuteSqlReader("/*SampleService:SampleMethod*/ SELECT * FROM [People] WHERE [Id] = @Id", new[] { new SqlParameter("@Id", 1) }, (reader) =>
            {
                var rows = 0;
                while (reader.Read())
                {
                    /* .... 
                    * Incredibly complex business logic that uses retrieved DB results that we wish to test...
                    * ...*/
                    rows++;
                    if (rows > 1)
                    {
                        throw new Exception("Query returned more than 1 record. Expected a single result by ID.");
                    }
                    var status = Convert.ToInt32(reader["Status"]);

                    if (status == 3)
                    {
                        // Do something special for status 3.
                    }
                }
            });
        }
        /*...*/
    }
}