using BS_KretaProjekt.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaTest
{
    public  class DbContextFactory
    {
        public static KretaDbContext Create()
        {
            //memory-ba van csak tárolva ameddig fut az egész
            var connection = new SqliteConnection("Data Source= :memory:");
            //legyen nyitva mindig, utána úgy is eldobja
            connection.Open();

            var options = new DbContextOptionsBuilder<KretaDbContext>().UseSqlite(connection).EnableSensitiveDataLogging().Options;

            var context = new KretaDbContext(options);

            //ha még nincs meg az adatbázis hozza létre
            context.Database.EnsureCreated();
            //legyenek adataink
            DbSeeder.Seed(context);
            //adjuk vissza a adatbázist
            return context;
        }
    }
}
