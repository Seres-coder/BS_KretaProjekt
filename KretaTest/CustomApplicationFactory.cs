using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace KretaTest
{
    public class CustomApplicationFactory
     : WebApplicationFactory<Program>
    {
        private SqliteConnection _connection = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Kiszedsz MINDENT, amit az AddDbContextPool (Npgsql) felrakhat
                services.RemoveAll<DbContextOptions<KretaDbContext>>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<KretaDbContext>();

                services.RemoveAll<IDbContextPool<KretaDbContext>>();
                services.RemoveAll<IScopedDbContextLease<KretaDbContext>>();
                services.RemoveAll<IDbContextFactory<KretaDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<KretaDbContext>>();

                // Sqlite in-memory (maradjon nyitva)
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();

                // Tesztben ne poolozz
                services.AddDbContext<KretaDbContext>(o => o.UseSqlite(_connection));

                // DB létrehozás
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
                db.Database.EnsureCreated();
                DbSeeder.Seed(db);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) _connection?.Dispose();
        }
    }
}
