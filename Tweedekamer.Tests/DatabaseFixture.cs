using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tweedekamer.Models;

public class DatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public DatabaseFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    public BlogPostContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BlogPostContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new BlogPostContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        _connection.Close();
    }
}
