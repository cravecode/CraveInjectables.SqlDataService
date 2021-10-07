# CraveInjectables SqlDataService
This Nuget package was inspired by the need to test legacy code that heavily used `ExecuteNonQuery`, `ExecuteScalar`, and `ExecuteReader` from the SqlCommand class. 
I needed to be able to test logic downstream of the ExecutreReader results, without being dependent on a database. 
I was able to turn the tightly coupled dependency on the SqlCommand class into a beneficial monitoring point by switching to this wrapper.

### Key objectives
* Minimal code changes to implement tests in code that is heavily coupled to the database.
* Simulate database response values to test downstream code.
* Audit/test `select`, `insert`, `update, and `delete` SQL generated from large code blocks that would normally not be testable without significant refactoring.


## Key Componenets
There are two main components to this nuget library. 
1. `SqlDataService` that is intended to be the piece of code that makes calls to your database instead of directly using the SqlCommand class.
2. `DataReaderMock` that is used to create a result set specific for your testing purposes.


## Getting Started

### Example problem code
Let's say you have business logic that is tightly coupled with your connection string and SqlCommand class. 
There is no way for us to get in between the business logic and data access for controlled tests.
```C#
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
```

### Refactored testable code

We'll refactor this code using dependency injection, to inject our SqlDataService class via the ISqlDataService interface. This allows us to inject a mocked SqlDataService during testing. 
I won't be covering dependency injection solutions for older and newer version of .NET. There are many to review such as: Autofac or Unity Container.
```C#
// The ISqlDataService via dependency injection...
ISqlDataService DataReaderService = new SqlDataService("Server=(localdb)\\ProjectsV13;Database=MyDatabase;Trusted_Connection=True;");

/*...*/

// Refactored previous code to use our SqlDataService as a wrapper to the SqlCommand and SqlDataReader.
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
/*...*/

```

## Example Moq




