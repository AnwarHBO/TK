using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweedekamer.Models;

namespace Tweedekamer.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly SqliteConnection connection;

        public BlogPostContext _context { get; private set; }

        public DatabaseFixture()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _context = new BlogPostContext(new DbContextOptionsBuilder<BlogPostContext>().UseSqlite(connection).Options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            connection.Dispose();
            _context.Dispose();
        }
    }


}
