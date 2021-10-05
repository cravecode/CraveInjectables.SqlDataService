using Xunit;

namespace CraveInjectables.SqlDataService.Tests
{
    public class SqlDataReaderServiceTests
    {
        [Fact]
        public void Test1()
        {
            string sqlConn = "Server=(localdb)\\ProjectsV13;Database=BillingService;Trusted_Connection=True;";
            var dataReaderService = new SqlDataService(sqlConn);
            dataReaderService.ExecuteSqlReader("select * from billing.status", null, (rdr) =>
            {
                while (rdr.Read())
                {
                    var name = rdr["Name"];
                    var id = rdr["Id"];
                }
                return true;
            });
        }
    }
}