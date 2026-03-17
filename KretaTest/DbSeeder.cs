using System.Text;

namespace BS_KretaProjekt.Persistence
{
    public static class DbSeeder
    {



        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public static void Seed(KretaDbContext db)
        {
            db.Database.EnsureCreated();

            if (db.Users.Any())
                return;

            // 1. Users first (independent)
            var users = new[]
            {
                new User { belepesnev = "admin", jelszo = HashPassword("admin123"), Role = "Admin" },
                new User { belepesnev = "tanar1", jelszo = HashPassword("tanar123"), Role = "Tanar" },
                new User { belepesnev = "diak1", jelszo = HashPassword("diak123"), Role = "Diak" }
            };
            db.Users.AddRange(users);
            db.SaveChanges();

            var adminUserId = users[0].user_id;
            var tanarUserId = users[1].user_id;
            var diakUserId = users[2].user_id;

            // 2. Independent entities: Osztalyok, Tantargyok
            var osztalyok = new[]
            {
                new Osztaly { osztaly_nev = "10.A" },
                new Osztaly { osztaly_nev = "11.B" }
            };
            db.Osztalyok.AddRange(osztalyok);
            db.SaveChanges();

            var osztaly1Id = osztalyok[0].osztaly_id;
            var osztaly2Id = osztalyok[1].osztaly_id;

            var tantargyok = new[]
            {
                new Tantargy { tantargy_nev = "Matematika" },
                new Tantargy { tantargy_nev = "Magyar" },
                new Tantargy { tantargy_nev = "Angol" }
            };
            db.Tantargyok.AddRange(tantargyok);
            db.SaveChanges();

            var matekId = tantargyok[0].tantargy_id;
            var magyarId = tantargyok[1].tantargy_id;
            var angolId = tantargyok[2].tantargy_id;

            // 3. Tanarok (needs user_id)
            var tanarok = new[]
            {
                new Tanar
                {
                    tanar_nev = "Kovács Tanár",
                    szak = "Matematika szakos",
                    tantargy_id = matekId,
                    Tantargy = tantargyok[0],
                    user_id = tanarUserId,
                    User = users[1],
                    Orarend = new List<Orarend>()
                }
            };
            db.Tanarok.AddRange(tanarok);
            db.SaveChanges();

            var tanar1Id = tanarok[0].tanar_id;

            // 4. Diakok (needs user_id, optional osztaly_id)
            var diakok = new[]
            {
                new Diak
                {
                    diak_nev = "Nagy Diák",
                    emailcim = "diak1@example.com",
                    osztaly_id = osztaly1Id,
                    Osztaly = osztalyok[0],
                    user_id = diakUserId,
                    User = users[2],
                    jegyek = new List<Jegy>()
                }
            };
            db.Diakok.AddRange(diakok);
            db.SaveChanges();

            var diak1Id = diakok[0].diak_id;

            // 5. Orarend (needs osztaly_id, tantargy_id, tanar_id)
            var orarendek = new[]
            {
                new Orarend
                {
                    osztaly_id = osztaly1Id,
                    Osztaly = osztalyok[0],
                    nap = DayOfWeek.Monday,
                    ora = 1,
                    tantargy_id = matekId,
                    Tantargy = tantargyok[0],
                    tanar_id = tanar1Id,
                    Tanar = tanarok[0]
                },
                new Orarend
                {
                    osztaly_id = osztaly1Id,
                    Osztaly = osztalyok[0],
                    nap = DayOfWeek.Monday,
                    ora = 2,
                    tantargy_id = magyarId,
                    Tantargy = tantargyok[1],
                    tanar_id = tanar1Id,
                    Tanar = tanarok[0]
                }
            };
            db.Orarendek.AddRange(orarendek);
            db.SaveChanges();

            // 6. Uzenetek (needs fogado_id=diak_id, user_id=tanar/admin)
            var uzenetek = new[]
            {
                new Uzenet
                {
                    tartalom = "Üdvözlöm a diákot!",
                    cim = "Üzenet diák1-nek",
                    fogado_id = diak1Id,
                    Fogado = diakok[0],
                    user_id = tanarUserId,
                    User = users[1],
                    kuldesidopontja = DateTimeOffset.Now.AddDays(-1)
                }
            };
            db.Uzenetek.AddRange(uzenetek);
            db.SaveChanges();

            // 7. Jegyek (needs tantargy_id, tanar_id, diak_id)
            var jegyek = new[]
            {
                new Jegy
                {
                    datum = DateTimeOffset.Now.AddDays(-10),
                    updatedatum = DateTimeOffset.Now.AddDays(-5),
                    ertek = 4,
                    tantargy_id = matekId,
                    Tantargy = tantargyok[0],
                    tanar_id = tanar1Id,
                    Tanar = tanarok[0],
                    diak_id = diak1Id,
                    Diak = diakok[0]
                },
                new Jegy
                {
                    datum = DateTimeOffset.Now.AddDays(-3),
                    updatedatum = DateTimeOffset.Now,
                    ertek = 5,
                    tantargy_id = angolId,
                    Tantargy = tantargyok[2],
                    tanar_id = tanar1Id,
                    Tanar = tanarok[0],
                    diak_id = diak1Id,
                    Diak = diakok[0]
                }
            };
            db.Jegyek.AddRange(jegyek);
            db.SaveChanges();

            // 8. Hianyzasok (independent)
            var hianyzasok = new[]
            {
                new Hianyzas { hianyzottorakszama = 2 },
                new Hianyzas { hianyzottorakszama = 0 }
            };
            db.Hianyzasok.AddRange(hianyzasok);
            db.SaveChanges();
        }
    }
}




