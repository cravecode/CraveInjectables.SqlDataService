# CraveInjectables SqlDataService
This Nuget package was inspired by the need to test legacy code that heavily used `ExecuteNonQuery`, `ExecuteScalar`, and `ExecuteReader` from the SqlCommand class. 
I needed to be able to test logic downstream of the `ExecuteReader` results, without being dependent on a database. 
I was able to turn the tightly coupled dependency on the SqlCommand class into a beneficial monitoring point by switching to this wrapper.

### Key objectives
* Minimal code changes to implement tests in code that is heavily coupled to the database.
* Simulate database response values to test downstream code.
* Audit/test `select`, `insert`, `update`, and `delete` SQL generated from large code blocks that would normally not be testable without significant refactoring.


## Key Components
There are two main components to this nuget library. 
1. `SqlDataService` that is intended to be the piece of code that makes calls to your database instead of directly using the SqlCommand class.
2. `DataReaderMock` that is used to create a result set specific for your testing purposes.


## Getting Started
Instead of writing in the order of: poor/untestable, improved code, the test. I have written these examples in reverse order. 
I'm doing this because most people want to be able to see the actual usage at the top, from a quick skim ;) Or at least I do.

#### High level documentation topics:
* Demonstrate testing code using a mocked SqlDataReader
* Showing how code can be improved by adding the ISqlDataService as a wrapper instead if tightly coupled to the SqlCommand class.
* Example of problematic/untestable code that is tightly coupled to the SqlCommand class.

### Example XUnit test using CraveInjectables SqlDataService and Moq
We'll create a XUnit test using the `SampleService` and `SampleMethod` that is now much more testable with our wrapper that allows us to get in between the use of our business logic and the `SqlCommand` class.

CraveInjectables SqlDataService nuget package also comes with a IDataReader mocking helper. 
We'll use this to mock our own results from a query provided as an array of anonymous class.

```csharp
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
```

### Refactored testable code
We'll refactor this code using dependency injection, to inject our SqlDataService class via the ISqlDataService interface. This allows us to inject a mocked SqlDataService during testing. 
I won't be covering dependency injection solutions for older and newer version of .NET. There are many to review such as: `Autofac` or `Unity Container`.

```csharp
// Assume the ISqlDataService was provided via dependency injection...
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
        // I also decided to add a SQL comment that can be used in my Moq param match.
        _SqlDataService.ExecuteSqlReader("/*SampleService:SampleMethod*/ SELECT [Id], [FirstName], [LastName], [Status], [Created] FROM [People] WHERE [Id] = @Id", new[] { new SqlParameter("@Id", 1) }, (reader) =>
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
```

### Example problem code
Let's say you have business logic that is tightly coupled with your connection string and SqlCommand class. 
There is no way for us to get in between the business logic and data access for controlled tests.
```csharp
public class SampleService {
    protected void SampleMethod()
    {
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
                        throw new Exception("Query returned more than 1 record. Expected a single result by ID.");
                    }
                    var status = Convert.ToInt32(reader["Status"]);

                    if (status == 3)
                    {
                        // Do something special for status 3.
                    }
                }
            }
        }
    }
}
```