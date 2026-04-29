using System.Threading.Tasks;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KretaTest
{
    public class DataModelTest
    {
        private readonly DataModel _model;
        private readonly KretaDbContext _context;
        
        public DataModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new DataModel(_context);
            DbSeeder.Seed(_context);
        }
        //Ellenõrzi, hogy a GetDiak metódus sikeresen visszaadja a diákok listáját, és tartalmazza a seed-elt "Nagy Diák" nevû diákot.
        [Fact]
        public async Task  GetDiak_Ok()
        {
           var result= await _model.GetDiak();
            Assert.True(_context.Diakok.Any());
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.diak_nev == "Nagy Diák");

        }
        //Ellenõrzi, hogy a GetDiak metódus InvalidOperationException-t dob, ha az adatbázisban nincs egyetlen diák sem (üres tábla).
        [Fact]
        public async Task GetDiak_ThrowsInvalidOperationException()
        {
            _context.Diakok.RemoveRange(_context.Diakok);
            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.GetDiak());
        }

        //Ellenõrzi, hogy a GetTeacher metódus visszaad legalább egy tanárt, és minden tanárnak van neve, ID-je és szaktárgya.
        [Fact]
        public async Task GetTeacher_Ok()
        {
            var result = await _model.GetTeacher();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.tanar_nev));
                Assert.True(x.tanar_id > 0);
                Assert.False(string.IsNullOrWhiteSpace(x.szak));
            });
        }
        //Ellenõrzi, hogy a GetTeacher metódus InvalidOperationException-t dob, ha nincs egyetlen tanár sem az adatbázisban.
        [Fact]
        public async Task GetTeacher_ThrowsInvalidOperationException()
        {
            _context.Tanarok.RemoveRange(_context.Tanarok);
            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.GetTeacher());
        }
        //Ellenõrzi, hogy a ModifyStudentData metódus sikeresen frissíti a diák adatait: a módosított név és email megjelenik az adatbázisban.
        [Fact]
        public async Task ModifyStudentData_Valid()
        {
            var eredetiDiak = _context.Diakok.First();

            var dto = new StudentDto
            {
                diak_id = eredetiDiak.diak_id,   
                diak_nev = "Módosított Név",
                emailcim = "uj_teszt@gmail.com",
                lakcim = "Új lakcím",
                osztaly_id = _context.Osztalyok.First().osztaly_id,
                szuletesi_datum = new DateTime(2009, 1, 1),
                szuloneve = "Új szülõnév"
            };

            await _model.ModifyStudentData(dto);

            var modified = await _context.Diakok
                .SingleAsync(x => x.diak_id == dto.diak_id);

            Assert.Equal(dto.diak_nev, modified.diak_nev);
            Assert.Equal(dto.emailcim, modified.emailcim);
        }
        //Ellenõrzi, hogy a ModifyStudentData metódus InvalidOperationException-t dob "Nincs minden adat megadva" üzenettel, ha a diak_nev mezõ üres.
        [Fact]
        public async Task ModifyStudentData_ThrowsInvalidOperation()
        {
            var eredetiDiak = _context.Diakok.First();

            var dto = new StudentDto
            {
                diak_id = eredetiDiak.diak_id,
                diak_nev = "",                      // hibás (üres)
                emailcim = "teszt@gmail.com",
                lakcim = "Teszt lakcím",
                osztaly_id = _context.Osztalyok.First().osztaly_id,
                szuletesi_datum = new DateTime(2009, 1, 1),
                szuloneve = "Teszt szülõ"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.ModifyStudentData(dto));
            Assert.Equal("Nincs minden adat megadva", ex.Message);
        }


        //Ellenõrzi, hogy a ModifyTeacherData metódus sikeresen frissíti a tanár nevét és szakját.
        [Fact]

        public async Task ModifyTeacherData_Valid()
        {
            var tanar = _context.Tanarok.First();
            var dto = new TeacherDto
            {
                szak="Informatika",
                tanar_nev="Belteki Anissza",
                tanar_id=tanar.tanar_id,
               

            };
            await _model.ModifyTeacherData(dto);

            var modifed = await _context.Tanarok.SingleAsync(x => x.tanar_id == tanar.tanar_id);

            Assert.Equal(dto.szak, modifed.szak);
            Assert.Equal(dto.tanar_nev, modifed.tanar_nev);
            Assert.Equal(dto.tanar_id, modifed.tanar_id);
        }
        //Ellenõrzi, hogy a ModifyTeacherData metódus InvalidOperationException-t dob, ha a tanar_nev mezõ üres.
        [Fact]
        public async Task ModifyTeacherData_ThrowsInvalidOperation()
        {
            var tanar = _context.Tanarok.First();

            var dto = new TeacherDto
            {
                tanar_id = tanar.tanar_id,
                tanar_nev = "",          // hibás
                szak = "Informatika"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.ModifyTeacherData(dto));
            Assert.Equal("Nincs minden adat megadva", ex.Message);
        }
        //Ellenõrzi, hogy a ModifyTeacherData metódus KeyNotFoundException-t dob, ha a megadott tanar_id nem létezik az adatbázisban.
        [Fact]
        public async Task ModifyTeacherData_ThrowsKeyNotFound()
        {
            // olyan ID, ami biztosan nem létezik
            var nonExistingId = _context.Tanarok.Any()
                ? _context.Tanarok.Max(t => t.tanar_id) + 999
                : 999999;

            var dto = new TeacherDto
            {
                tanar_id = nonExistingId,
                tanar_nev = "Teszt Tanár",
                szak = "Informatika"
            };

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _model.ModifyTeacherData(dto));
            Assert.Equal("Nincs ilyen tanar", ex.Message);
        }

        //Ellenõrzi, hogy a DeleteStudentData metódus sikeresen törli a diákot: a rekordszám eggyel csökken, és az adott ID többé nem található.
        [Fact]
        public async Task DeleteStudentData_Valid()
        {
            var student = _context.Diakok.First();
            var before_count = _context.Diakok.Count();

            await _model.DeleteStudentData(student.diak_id);

            Assert.Equal(before_count - 1, _context.Diakok.Count());
            Assert.False(_context.Diakok.Any(x => x.diak_id == student.diak_id));
        }
        //Ellenõrzi, hogy a DeleteStudentData metódus InvalidOperationException-t dob "nincs ilyen diak" üzenettel, ha a megadott ID nem létezik.
        [Fact]
        public async Task DeleteStudentData_ThrowsInvalidOperation()
        {
            // olyan ID, ami biztosan nem létezik
            var nonExistingId = _context.Diakok.Any()
                ? _context.Diakok.Max(d => d.diak_id) + 999
                : 999999;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.DeleteStudentData(nonExistingId));
            Assert.Equal("nincs ilyen diak", ex.Message);
        }
        //Ellenõrzi, hogy a DeleteTeacherData metódus sikeresen törli a tanárt: a rekordszám eggyel csökken, és az adott ID többé nem található.
        [Fact]
        public async Task DeleteTeacherData_Valid()
        {
            var teacher = _context.Tanarok.First();
            var before_count = _context.Tanarok.Count();

            await _model.DeleteTeacherData(teacher.tanar_id);

            Assert.Equal(before_count - 1, _context.Tanarok.Count());
            Assert.False(_context.Tanarok.Any(x => x.tanar_id == teacher.tanar_id));
        }



    }
}