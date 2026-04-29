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
        //Létrehoz egy tesztadatokkal feltöltött (seeded) in-memory adatbázis kontextust. A kapcsolat nyitva marad az egész életciklus alatt, majd a GC eldobja. Használd, ha a tesztnek szüksége van előre betöltött adatokra.
        public static KretaDbContext Create()
        {

            //memory-ba van csak tárolva ameddig fut az egész
            var connection = new SqliteConnection("Data Source=:memory:");
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
        //Létrehoz egy üres (seed nélküli) in-memory adatbázis kontextust. Használd, ha a teszt maga szeretné kontrolálni a kezdőállapotot, vagy üres adatbázison szeretné ellenőrizni a hibaágakat.
        public static KretaDbContext CreateEmpty()
        {

            //memory-ba van csak tárolva ameddig fut az egész
            var connection = new SqliteConnection("Data Source=:memory:");
            //legyen nyitva mindig, utána úgy is eldobja
            connection.Open();

            var options = new DbContextOptionsBuilder<KretaDbContext>().UseSqlite(connection).EnableSensitiveDataLogging().Options;

            var context = new KretaDbContext(options);

            //ha még nincs meg az adatbázis hozza létre
            context.Database.EnsureCreated();
            //legyenek adataink

            //adjuk vissza a adatbázist
            return context;
        }

    }
}
