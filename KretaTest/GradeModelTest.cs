using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaTest
{
    public class GradeModelTest
    {
        private readonly GradeModel _model;
        private readonly KretaDbContext _context;

        public GradeModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new GradeModel(_context);
        }
        //Ellenőrzi, hogy az AddNewGrade metódus sikeresen hozzáad egy új jegyet, és a jegy a megfelelő diákhoz, tanárhoz és tantárgyhoz van rendelve. A rekordszám eggyel nő.
        [Fact]
        public async Task AddNewGrade_Valid()
        {
            var beforeCount = _context.Jegyek.Count();
            var dto = new GradeAdd
            {
                diak_nev = "Nagy Diák",
                ertek = 4,
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika"
            };

            await _model.AddNewGrade(dto);

            Assert.Equal(beforeCount + 1, _context.Jegyek.Count());

            var ujJegy = _context.Jegyek
                .Where(j => j.ertek == dto.ertek)
                .Include(j => j.Diak)
                .Include(j => j.Tanar)
                .Include(j => j.Tantargy)
                .FirstOrDefault();

            Assert.NotNull(ujJegy);
            Assert.Equal(dto.diak_nev, ujJegy.Diak.diak_nev);
            Assert.Equal(dto.tanar_nev, ujJegy.Tanar.tanar_nev);
            Assert.Equal(dto.tantargy_nev, ujJegy.Tantargy.tantargy_nev);
        }
        //Ellenőrzi, hogy az AddNewGrade metódus InvalidOperationException-t dob "Nincs minden adat megadva" üzenettel, ha az ertek = 0.
        [Fact]
        public async Task AddNewGrade_ThrowsInvalidOperation()
        {
            var dto = new GradeAdd
            {
                diak_nev = "Nagy Diák",
                ertek = 0, // <-- trigger
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.AddNewGrade(dto));
            Assert.Equal("Nincs minden adat megadva", ex.Message);
        }
        //Ellenőrzi, hogy a GradeModify metódus sikeresen frissíti a jegy értékét és dátumát az adatbázisban.
        [Fact]
        public async Task GradeModify_Valid()
        {
            var grade = _context.Jegyek.First();
            var dto = new GradeModify
            {
                ertek = 1,
                jegy_id = grade.jegy_id,
                updatedatum = DateTimeOffset.Now.AddDays(-5),


            };
            await _model.GradeModify(dto);
            var modifed = await _context.Jegyek.SingleAsync(x => x.jegy_id == grade.jegy_id);

            Assert.Equal(dto.ertek, modifed.ertek);
            Assert.Equal(dto.jegy_id, modifed.jegy_id);
            Assert.Equal(dto.updatedatum, modifed.updatedatum);
        }
        //Ellenőrzi, hogy a GradeModify metódus InvalidOperationException-t dob "Nincs minden adat megadva" üzenettel, ha az ertek = 0.
        [Fact]
        public async Task GradeModify_ThrowsInvalidOperation()
        {
            var grade = _context.Jegyek.First();

            var dto = new GradeModify
            {
                jegy_id = grade.jegy_id,
                ertek = 0, // <-- trigger
                updatedatum = DateTimeOffset.Now
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.GradeModify(dto));
            Assert.Equal("Nincs minden adat megadva", ex.Message);
        }

        //Ellenőrzi, hogy a DeleteGrade metódus sikeresen törli a jegyet: a rekordszám eggyel csökken.
        [Fact]
        public async Task GradeDelete_Valid()
        {
            var before_count = _context.Jegyek.Count();
            var id = _context.Jegyek.First().jegy_id;
            await _model.DeleteGrade(id);
            Assert.Equal(before_count - 1, _context.Jegyek.Count());
        }
        [Fact]
        //Ellenőrzi, hogy a DeleteGrade metódus KeyNotFoundException-t dob "Nincs ilyen jegy" üzenettel, ha a megadott ID nem létezik.
        public async Task GradeDelete_ThrowsKeyNotFound()
        {
            // olyan ID, ami biztosan nem létezik
            var nonExistingId = _context.Jegyek.Any()
                ? _context.Jegyek.Max(j => j.jegy_id) + 999
                : 999999;

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _model.DeleteGrade(nonExistingId));
            Assert.Equal("Nincs ilyen jegy", ex.Message);
        }
        //Ellenőrzi, hogy az AllGrades metódus diák ID alapján visszaadja a jegyeket. A seed-elt diáknak (ID=1) pontosan 2 jegye van.
        [Fact]
        public void AllGradesByStudent_Valid()
        {
            var grades_student = _model.AllGrades(1, 0);
            Assert.NotEmpty(grades_student);
            Assert.Equal(2, grades_student.Count());
        }
        //Ellenőrzi, hogy az AllGrades metódus tanár ID alapján visszaadja a jegyeket. A seed-elt tanárnak (ID=1) pontosan 2 jegye van
        [Fact]
        public void AllGradesByTeacher_Valid()
        {
            var grades_student = _model.AllGrades(0, 1);
            Assert.NotEmpty(grades_student);
            Assert.Equal(2, grades_student.Count());

        }
        //Ellenőrzi, hogy az AllGrades metódus InvalidOperationException-t dob, ha mindkét ID (diák és tanár) meg van adva egyszerre.
        [Fact]
        public void AllGrades_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _model.AllGrades(1, 1));
            Assert.Equal("Nem lehet egyszerre diák és tanár id alapján keresni", ex.Message);
        }
    }
}
