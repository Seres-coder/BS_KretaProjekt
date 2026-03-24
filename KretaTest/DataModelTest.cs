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

        [Fact]
        public async Task  GetDiak_Ok()
        {
           var result= await _model.GetDiak();
            Assert.True(_context.Diakok.Any());
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.diak_nev == "Nagy Diák");

        }

        [Fact]
        public async Task GetDiak_ThrowsInvalidOperationException()
        {
            _context.Diakok.RemoveRange(_context.Diakok);
            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.GetDiak());
        }


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

        [Fact]
        public async Task GetTeacher_ThrowsInvalidOperationException()
        {
            _context.Tanarok.RemoveRange(_context.Tanarok);
            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(() => _model.GetTeacher());
        }

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

        [Fact]
        public async Task ModifyStudentData_ThrowsKeyNotFound()
        {
            var eredetiDiak = _context.Diakok.First();

            // olyan osztaly_id, ami biztosan nem létezik
            var nonExistingOsztalyId = _context.Osztalyok.Any()
                ? _context.Osztalyok.Max(o => o.osztaly_id) + 999
                : 999999;

            var dto = new StudentDto
            {
                diak_id = eredetiDiak.diak_id,
                diak_nev = "Teszt Név",
                emailcim = "teszt@gmail.com",
                lakcim = "Teszt lakcím",
                osztaly_id = nonExistingOsztalyId,  // <-- ez fogja triggerelni
                szuletesi_datum = new DateTime(2009, 1, 1),
                szuloneve = "Teszt szülõ"
            };

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _model.ModifyStudentData(dto));
            Assert.Equal("Nincs ilyen diak", ex.Message); 
        }


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


        [Fact]
        public async Task DeleteStudentData_Valid()
        {
            var student = _context.Diakok.First();
            var before_count = _context.Diakok.Count();

            await _model.DeleteStudentData(student.diak_id);

            Assert.Equal(before_count - 1, _context.Diakok.Count());
            Assert.False(_context.Diakok.Any(x => x.diak_id == student.diak_id));
        }

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

        [Fact]
        public async Task DeleteTeacherData_Valid()
        {
            var teacher = _context.Tanarok.First();
            var before_count = _context.Tanarok.Count();

            await _model.DeleteTeacherData(teacher.tanar_id);

            Assert.Equal(before_count - 1, _context.Tanarok.Count());
            Assert.False(_context.Tanarok.Any(x => x.tanar_id == teacher.tanar_id));
        }



        [Fact]
        public async Task DeleteTeacherData_ThrowsInvalidOperation()
        {
            // olyan ID, ami biztosan nem létezik
            var nonExistingId = _context.Tanarok.Any()
                ? _context.Tanarok.Max(d => d.tanar_id) + 999
                : 999999;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.DeleteTeacherData(nonExistingId));
            Assert.Equal("nincs ilyen tanar", ex.Message);
        }
    }
}