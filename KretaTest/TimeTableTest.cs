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
    public class TimeTableTest
    {
        private readonly TimeTableModel _model;
        private readonly KretaDbContext _context;

        public TimeTableTest()
        {
            _context = DbContextFactory.Create();
            _model = new TimeTableModel(_context);
        }
        [Fact]
        public async Task CreateTimeTable_Valid()
        {
            var before_count = _context.Orarendek.Count();
            var dto = new CreateOrarendDto
            {
                osztaly_id = 1,
                nap = DayOfWeek.Monday,
                ora = 1,
                tantargy = "Matematika",
                Tanarnev = "Kovács János"
            };
            await _model.CreateTimeTable(dto);
            var after_count = _context.Orarendek.Count();
            Assert.Equal(before_count + 1, after_count);
        }
        [Fact]
        public async Task ModifyTimeTable_Valid()
        {
            var orarend = _context.Orarendek.First();
            var dto = new UpdateOrarendDto
            {
                orarend_id = orarend.orarend_id,
                osztaly_nev = "9.A",
                nap = DayOfWeek.Tuesday,
                ora = 2,
                tantargy_nev = "Informatika",
                tanar_nev = "Nagy Anna"
            };
            await _model.ModifyTimeTable(dto);
            var modified_orarend = _context.Orarendek.First(x => x.orarend_id == orarend.orarend_id);
            Assert.Equal(2, modified_orarend.ora);
            Assert.Equal(DayOfWeek.Tuesday, modified_orarend.nap);
        }
        [Fact]
        public async Task DeleteTimeTable()
        {
            var orarend = _context.Orarendek.First();
            var before_count = _context.Orarendek.Count();

            await _model.DeleteTimeTable(orarend.orarend_id);

            Assert.Equal(before_count - 1, _context.Orarendek.Count());
            Assert.False(_context.Orarendek.Any(x => x.orarend_id == orarend.orarend_id));
        }
    }
}
