namespace BS_KretaProjekt.Persistence
{
    public static class DbSeeder
    {

        public static void Seed(KretaDbContext context)
        {
            context.Database.EnsureCreated();

            // Ha már van adat, ne seedeljen újra
            if (context.Users.Any())
                return;

            // =========================
            // OSZTÁLY
            // =========================
            var osztaly = new Osztaly
            {
                osztaly_nev = "10.A"
            };

            context.Osztalyok.Add(osztaly);
            context.SaveChanges();

            // =========================
            // TANTÁRGYAK
            // =========================
            var matek = new Tantargy { tantargy_nev = "Matematika" };
            var info = new Tantargy { tantargy_nev = "Informatika" };

            context.Tantargyok.AddRange(matek, info);
            context.SaveChanges();

            // =========================
            // USER - TANÁR
            // =========================
            var tanarUser = new User
            {
                belepesnev = "tanar1",
                jelszo = "1234",
                Role = "Tanar"
            };

            context.Users.Add(tanarUser);
            context.SaveChanges();

            var tanar = new Tanar
            {
                tanar_nev = "Kiss Péter",
                szak = "Informatika",
                tantargy_id = info.tantargy_id,
                user_id = tanarUser.user_id
            };

            context.Tanarok.Add(tanar);
            context.SaveChanges();

            // =========================
            // USER - DIÁK
            // =========================
            var diakUser = new User
            {
                belepesnev = "diak1",
                jelszo = "1234",
                Role = "Diak"
            };

            context.Users.Add(diakUser);
            context.SaveChanges();

            var diak = new Diak
            {
                diak_nev = "Nagy Anna",
                user_id = diakUser.user_id,
                osztaly_id = osztaly.osztaly_id,
                szuletesi_datum = new DateTime(2008, 5, 10),
                lakcim = "Budapest",
                szuloneve = "Nagy Éva",
                emailcim = "anna@test.hu"
            };

            context.Diakok.Add(diak);
            context.SaveChanges();

            // =========================
            // JEGYEK
            // =========================
            var jegy1 = new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 5,
                tantargy_id = info.tantargy_id,
                tanar_id = tanar.tanar_id,
                diak_id = diak.diak_id
            };

            var jegy2 = new Jegy
            {
                datum = DateTimeOffset.Now,
                updatedatum = DateTimeOffset.Now,
                ertek = 4,
                tantargy_id = matek.tantargy_id,
                tanar_id = tanar.tanar_id,
                diak_id = diak.diak_id
            };

            context.Jegyek.AddRange(jegy1, jegy2);
            context.SaveChanges();

            // =========================
            // ÜZENET
            // =========================
            var uzenet = new Uzenet
            {
                cim = "Dolgozat",
                tartalom = "Holnap dolgozat lesz.",
                fogado_id = diak.diak_id,
                user_id = tanarUser.user_id,
                kuldesidopontja = DateTimeOffset.Now
            };

            context.Uzenetek.Add(uzenet);
            context.SaveChanges();
        }
    }
}

