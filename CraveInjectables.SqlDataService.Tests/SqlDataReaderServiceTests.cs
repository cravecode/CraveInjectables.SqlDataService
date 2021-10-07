using System;
using System.Data.SqlClient;
using Xunit;

namespace CraveInjectables.SqlDataService.Tests
{
    public class SqlDataReaderServiceTests
    {
        [Fact]
        public void Test1()
        {
            string sqlConn = "Server=(localdb)\\ProjectsV13;Database=MyDatabase;Trusted_Connection=True;";
            ISqlDataService DataReaderService = new SqlDataService(sqlConn);

            DataReaderService.ExecuteSqlReader("SELECT TOP 1 * FROM [People] WHERE [Status] = @StatusId", new[] { new SqlParameter("@Id", 1) }, (reader) =>
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
                        // Throw an exception!
                    }
                    var status = Convert.ToInt32(reader["Status"]);

                    if(status == 3)
                    {
                        // Do something special for status 3.
                    }
                }
            });


			using (var connection = new SqlConnection("Server=(localdb)\\ProjectsV13;Database=MyDatabase;Trusted_Connection=True;"))
			using (var command = new SqlCommand(
					"SELECT [Id], [FirstName], [LastName], [Status], [Created] FROM [People] WHERE [Id] = @Id",
					connection))
			{
				command.Parameters.Add(new SqlParameter("@Id", 1));
				connection.Open();

                var rows = 0;

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
                        /* .... 
                         * Incredibly complex business logic that uses retrieved DB results that we wish to test...
                         * ...*/
						rows++;
						if (rows > 1)
						{
							// Throw an exception!
						}
                        var status = Convert.ToInt32(reader["Status"]);

						if(status == 3)
                        {
							// Do something special for status 3.
                        }
					}
				}
			}
        }
    }
}