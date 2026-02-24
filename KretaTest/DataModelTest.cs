using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;

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
        }
       

        [Fact]
        public void GetDiak()
        {
            var result = _model.GetDiak();

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.diak_nev));
                Assert.True(x.user_id > 0);
                Assert.True(x.osztaly_id > 0);
                Assert.False(string.IsNullOrWhiteSpace(x.emailcim));
            });
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

        // modifyingot nem tudom lol

        [Fact]
        public async Task AddStudentData_Valid()
        {
            var before_count = _context.Diakok.Count();

            var dto = new StudentDto
            {
                diak_nev = "Kiss Bence",
                osztaly_id = 1,
                lakcim = "Budapest",
                szuloneve = "Kiss Éva",
                emailcim = "bence@email.com",
                szuletesi_datum = new DateTime(2008, 5, 10)
            };

            await _model.AddStudentData(dto);

            Assert.Equal(before_count + 1, _context.Diakok.Count());
            Assert.True(_context.Diakok.Any(x => x.diak_nev == "Kiss Bence"));
        }

        [Fact]

        public async Task AddTeacherData_Valid()
        {
            var before_count = _context.Tanarok.Count();
            var dto = new TeacherDto
            {
                tanar_nev = "Kovács Péter",
                szak = "Matematika"
            };
            await _model.AddTeacherData(dto);
            Assert.Equal(before_count + 1, _context.Tanarok.Count());
            Assert.True(_context.Tanarok.Any(x => x.tanar_nev == "Kovács Péter"));

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