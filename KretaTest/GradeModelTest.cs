using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
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
        public async Task GradeAdd()
        {
            var before_count = _context.Jegyek.Count();
            var dto = new GradeAdd
            {
                diak_nev = "Kiss Bence",
                ertek = 1,
                tanar_nev = "Nagy Péter",
                tantargy_nev = "Matematika"
            };
            await _model.AddNewGrade(dto);
            Assert.Equal(before_count + 1, _context.Tanarok.Count());
            Assert.True(_context.Tanarok.Any(x => x.tanar_nev == ""));
        }
        //modify no

        [Fact]
        public async Task GradeDelete()
        {
            var before_count = _context.Jegyek.Count();
            var id = _context.Jegyek.First().jegy_id;
            await _model.DeleteGrade(id);
            Assert.Equal(before_count - 1, _context.Jegyek.Count());
        }
        // ez tul nehez help kell
    }
}
