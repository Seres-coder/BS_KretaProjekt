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
        public void GetDiak()
        {
           var result=_model.GetDiak();
            Assert.True(_context.Diakok.Any());
            Assert.NotEmpty(result);
            Assert.Contains(result, x => x.diak_nev == "Nagy Diák");

        }

        [Fact]
        public void GetTeacher()
        {
            var result = _model.GetTeacher();
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
        public async Task DeleteStudentData_Valid()
        {
            var student = _context.Diakok.First();
            var before_count = _context.Diakok.Count();

            await _model.DeleteStudentData(student.diak_id);

            Assert.Equal(before_count - 1, _context.Diakok.Count());
            Assert.False(_context.Diakok.Any(x => x.diak_id == student.diak_id));
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
        
    }
}