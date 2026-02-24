namespace BS_KretaProjekt.Persistence
{
    public static class DbSeeder
    {



        public static void Seed(KretaDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return; // Már van adat

            // =========================
            // USERS
            // =========================
            var adminUser = new User { belepesnev = "admin", jelszo = "1234", Role = "Admin" };
            var tanarUser1 = new User { belepesnev = "tanar1", jelszo = "1234", Role = "Tanar" };
            var tanarUser2 = new User { belepesnev = "tanar2", jelszo = "1234", Role = "Tanar" };
            var diakUser1 = new User { belepesnev = "diak1", jelszo = "1234", Role = "Diak" };
            var diakUser2 = new User { belepesnev = "diak2", jelszo = "1234", Role = "Diak" };

            context.Users.AddRange(adminUser, tanarUser1, tanarUser2, diakUser1, diakUser2);
            context.SaveChanges();

            // =========================
            // OSZTÁLYOK
            // =========================
            var osztaly10A = new Osztaly { osztaly_nev = "10.A" };
            var osztaly10B = new Osztaly { osztaly_nev = "10.B" };
            context.Osztalyok.AddRange(osztaly10A, osztaly10B);
            context.SaveChanges();

            // =========================
            // TANTÁRGYAK
            // =========================
            var matek = new Tantargy { tantargy_nev = "Matematika", Orarend = new List<Orarend>() };
            var info = new Tantargy { tantargy_nev = "Informatika", Orarend = new List<Orarend>() };
            var fizika = new Tantargy { tantargy_nev = "Fizika", Orarend = new List<Orarend>() };
            context.Tantargyok.AddRange(matek, info, fizika);
            context.SaveChanges();

            // =========================
            // TANÁROK
            // =========================
            var tanar1 = new Tanar
            {
                tanar_nev = "Kiss Péter",
                szak = "Matematika",
                User = tanarUser1,
                Tantargy = matek,
                jegyek = new List<Jegy>(),
                Orarend = new List<Orarend>()
            };
            var tanar2 = new Tanar
            {
                tanar_nev = "Nagy Eszter",
                szak = "Informatika",
                User = tanarUser2,
                Tantargy = info,
                jegyek = new List<Jegy>(),
                Orarend = new List<Orarend>()
            };
            context.Tanarok.AddRange(tanar1, tanar2);
            context.SaveChanges();

            // =========================
            // DIÁKOK
            // =========================
            var diak1 = new Diak
            {
                diak_nev = "Nagy Anna",
                User = diakUser1,
                Osztaly = osztaly10A,
                szuletesi_datum = new DateTime(2008, 5, 10),
                lakcim = "Budapest",
                szuloneve = "Nagy Éva",
                emailcim = "anna@test.hu",
                jegyek = new List<Jegy>()
            };
            var diak2 = new Diak
            {
                diak_nev = "Kovács Bence",
                User = diakUser2,
                Osztaly = osztaly10B,
                szuletesi_datum = new DateTime(2008, 7, 20),
                lakcim = "Debrecen",
                szuloneve = "Kovács Erika",
                emailcim = "bence@test.hu",
                jegyek = new List<Jegy>()
            };
            context.Diakok.AddRange(diak1, diak2);
            context.SaveChanges();

            // =========================
            // ÓRAREND
            // =========================
            var orarend1 = new Orarend
            {
                osztaly = osztaly10A,
                nap = DayOfWeek.Monday,
                ora = 1,
                tantargy = matek,
                Tanar = tanar1
            };
            var orarend2 = new Orarend
            {
                osztaly = osztaly10A,
                nap = DayOfWeek.Monday,
                ora = 2,
                tantargy = info,
                Tanar = tanar2
            };
            var orarend3 = new Orarend
            {
                osztaly = osztaly10B,
                nap = DayOfWeek.Tuesday,
                ora = 1,
                tantargy = fizika,
                Tanar = tanar1
            };
            context.Orarendek.AddRange(orarend1, orarend2, orarend3);
            context.SaveChanges();

            // =========================
            // JEGYEK
            // =========================
            var jegy1 = new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 5,
                tantargy = matek,
                Tanar = tanar1,
                Diak = diak1
            };
            var jegy2 = new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 4,
                tantargy = info,
                Tanar = tanar2,
                Diak = diak1
            };
            var jegy3 = new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 3,
                tantargy = fizika,
                Tanar = tanar1,
                Diak = diak2
            };
            context.Jegyek.AddRange(jegy1, jegy2, jegy3);
            context.SaveChanges();

            // =========================
            // ÜZENETEK
            // =========================
            var uzenet1 = new Uzenet
            {
                cim = "Dolgozat",
                tartalom = "Holnap dolgozat lesz.",
                Fogado = diak1,
                User = tanarUser1,
                kuldesidopontja = DateTimeOffset.Now
            };
            var uzenet2 = new Uzenet
            {
                cim = "Hiányzás",
                tartalom = "Kérlek pótold az órát.",
                Fogado = diak2,
                User = tanarUser2,
                kuldesidopontja = DateTimeOffset.Now
            };
            context.Uzenetek.AddRange(uzenet1, uzenet2);
            context.SaveChanges();

            // =========================
            // HIÁNYZÁSOK
            // =========================
            var hianyzas1 = new Hianyzas { hianyzottorakszama = 2 };
            var hianyzas2 = new Hianyzas { hianyzottorakszama = 3 };
            context.Hianyzasok.AddRange(hianyzas1, hianyzas2);
            context.SaveChanges();
        }
    }
}



