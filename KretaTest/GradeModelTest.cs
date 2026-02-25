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
                .Include(j => j.tantargy)
                .FirstOrDefault();

            Assert.NotNull(ujJegy);
            Assert.Equal(dto.diak_nev, ujJegy.Diak.diak_nev);
            Assert.Equal(dto.tanar_nev, ujJegy.Tanar.tanar_nev);
            Assert.Equal(dto.tantargy_nev, ujJegy.tantargy.tantargy_nev);
        }

        [Fact]
        public async Task GradeModify_Valid()
        {
            var grade = _context.Jegyek.First();
            var dto = new GradeModify
            {
               ertek = 1,
               jegy_id=grade.jegy_id,
               updatedatum= DateTimeOffset.Now.AddDays(-5),


            };
            await _model.GradeModify(dto);
            var modifed = await _context.Jegyek.SingleAsync(x => x.jegy_id == grade.jegy_id);

            Assert.Equal(dto.ertek, modifed.ertek);
            Assert.Equal(dto.jegy_id, modifed.jegy_id);
            Assert.Equal(dto.updatedatum, modifed.updatedatum);
        }


        [Fact]
        public async Task GradeDelete()
        {
            var before_count = _context.Jegyek.Count();
            var id = _context.Jegyek.First().jegy_id;
            await _model.DeleteGrade(id);
            Assert.Equal(before_count - 1, _context.Jegyek.Count());
        }
        [Fact]
        public void AllGradesByStudent()
        {
            var grades_student = _model.AllGrades(1, 0);
            Assert.NotEmpty(grades_student);
            Assert.Equal(2, grades_student.Count());
        }
        [Fact]
        public void AllGradesByTeacher()
        {
            var grades_student = _model.AllGrades(0, 1);
            Assert.NotEmpty(grades_student);
            Assert.Equal(2, grades_student.Count());

        }
    }
}
