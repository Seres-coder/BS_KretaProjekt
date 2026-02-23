namespace BS_KretaProjekt.Persistence
{
    public static class DbSeeder
    {
        public static void Seed(KretaDbContext db)
        {
            if (db.Users.Any()) return;

            // ===================== USERS =====================
            var users = new List<User>
        {
            new User { belepesnev = "admin", jelszo = "admin123", Role = "Admin" },
            new User { belepesnev = "tanar1", jelszo = "tanar123", Role = "Tanar" },
            new User { belepesnev = "tanar2", jelszo = "tanar123", Role = "Tanar" },
            new User { belepesnev = "diak1", jelszo = "diak123", Role = "Diak" },
            new User { belepesnev = "diak2", jelszo = "diak123", Role = "Diak" }
        };

            db.Users.AddRange(users);
            db.SaveChanges();


            // ===================== OSZTÁLYOK =====================
            var osztalyok = new List<Osztaly>
        {
            new Osztaly { osztaly_nev = "9.A" },
            new Osztaly { osztaly_nev = "10.B" }
        };

            db.Osztalyok.AddRange(osztalyok);
            db.SaveChanges();


            // ===================== TANTÁRGYAK =====================
            var tantargyak = new List<Tantargy>
        {
            new Tantargy { tantargy_nev = "Matematika" },
            new Tantargy { tantargy_nev = "Magyar" },
            new Tantargy { tantargy_nev = "Informatika" }
        };

            db.Tantargyok.AddRange(tantargyak);
            db.SaveChanges();


            // ===================== TANÁROK =====================
            var tanarok = new List<Tanar>
        {
            new Tanar
            {
                tanar_nev = "Kovács Péter",
                szak = "Matematika",
                user_id = 2,
                tantargy_id = 1
            },
            new Tanar
            {
                tanar_nev = "Nagy Anna",
                szak = "Informatika",
                user_id = 3,
                tantargy_id = 3
            }
        };

            db.Tanarok.AddRange(tanarok);
            db.SaveChanges();


            // ===================== DIÁKOK =====================
            var diakok = new List<Diak>
        {
            new Diak
            {
                diak_nev = "Kiss Bence",
                user_id = 4,
                osztaly_id = 1,
                tanar_id = 1,
                szuletesi_datum = new DateTime(2008, 5, 10),
                lakcim = "Budapest",
                szuloneve = "Kiss Éva",
                emailcim = "bence@email.com"
            },
            new Diak
            {
                diak_nev = "Tóth Lili",
                user_id = 5,
                osztaly_id = 2,
                tanar_id = 2,
                szuletesi_datum = new DateTime(2007, 9, 21),
                lakcim = "Debrecen",
                szuloneve = "Tóth Mária",
                emailcim = "lili@email.com"
            }
        };

            db.Diakok.AddRange(diakok);
            db.SaveChanges();


            // ===================== JEGYEK =====================
            var jegyek = new List<Jegy>
        {
            new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 5,
                tantargy_id = 1,
                tanar_id = 1,
                diak_id = 1
            },
            new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 4,
                tantargy_id = 3,
                tanar_id = 2,
                diak_id = 2
            }
        };

            db.Jegyek.AddRange(jegyek);
            db.SaveChanges();


            // ===================== ÜZENETEK =====================
            var uzenetek = new List<Uzenet>
        {
            new Uzenet
            {
                cim = "Dolgozat",
                tartalom = "Holnap matematika dolgozat lesz.",
                fogado_id = 1,
                user_id = 2,
                kuldesidopontja = DateTimeOffset.Now
            },
            new Uzenet
            {
                cim = "Projekt",
                tartalom = "Az informatika projekt jövő hétre kész legyen.",
                fogado_id = 2,
                user_id = 3,
                kuldesidopontja = DateTimeOffset.Now
            }
        };

            db.Uzenetek.AddRange(uzenetek);
            db.SaveChanges();


            // ===================== ÓRAREND =====================
            var orarend = new List<Orarend>
        {
            new Orarend
            {
                osztaly_id = 1,
                nap = DayOfWeek.Monday,
                ora = 1,
                tantargy_id = 1,
                tanar_id = 1
            },
            new Orarend
            {
                osztaly_id = 2,
                nap = DayOfWeek.Tuesday,
                ora = 2,
                tantargy_id = 3,
                tanar_id = 2
            }
        };

            db.Orarendek.AddRange(orarend);
            db.SaveChanges();


            // ===================== HIÁNYZÁS =====================
            var hianyzasok = new List<Hianyzas>
        {
            new Hianyzas { hianyzottorakszama = 2 },
            new Hianyzas { hianyzottorakszama = 5 }
        };

            db.Hianyzasok.AddRange(hianyzasok);
            db.SaveChanges();
        }
    }
}
