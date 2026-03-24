using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Model
{
    public class TimeTableModel
    {
        private readonly KretaDbContext _context;
        public TimeTableModel(KretaDbContext context)
        {
            _context = context;
        }

        #region Create TimeTable
        public async Task CreateTimeTable(CreateOrarendDto dto)
        { 
            if(string.IsNullOrWhiteSpace(dto.tantargy) || string.IsNullOrWhiteSpace(dto.Tanarnev) || dto.osztaly_id == 0 || dto.nap == DayOfWeek.Sunday || dto.nap == DayOfWeek.Saturday || dto.ora <= 0)
                throw new InvalidOperationException("Nincs minden adat megadva");

            var tantargyId = _context.Tantargyok.First(x => x.tantargy_nev == dto.tantargy).tantargy_id;
            var tanarId = _context.Tanarok.First(x => x.tanar_nev == dto.Tanarnev).tanar_id;

            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Orarendek.Add(new Orarend
                {
                    osztaly_id = dto.osztaly_id,
                    nap = dto.nap,
                    ora = dto.ora,
                    tantargy_id = tantargyId,
                    tanar_id = tanarId
                });

                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }

            await Task.CompletedTask;
        }
#endregion

        #region Modify TimeTable
        public async Task ModifyTimeTable(UpdateOrarendDto dto)
        {

            if(string.IsNullOrWhiteSpace(dto.tantargy_nev) || string.IsNullOrWhiteSpace(dto.tanar_nev) || string.IsNullOrWhiteSpace(dto.osztaly_nev) || dto.nap == DayOfWeek.Sunday || dto.nap == DayOfWeek.Saturday || dto.ora <= 0)
                throw new InvalidOperationException("Nincs minden adat megadva");

            var tantargyId = _context.Tantargyok.First(x => x.tantargy_nev == dto.tantargy_nev).tantargy_id;
            var tanarId = _context.Tanarok.First(x => x.tanar_nev == dto.tanar_nev).tanar_id;
            var osztalyid= _context.Osztalyok.First(x=>x.osztaly_nev==dto.osztaly_nev).osztaly_id;
            if (!_context.Orarendek.Any(x => x.orarend_id == dto.orarend_id))
                throw new InvalidCastException("Nincs ilyen órarend!");

            using (var trx = _context.Database.BeginTransaction())
            {
                var orarend = _context.Orarendek
                    .First(x => x.orarend_id == dto.orarend_id);

                orarend.osztaly_id = osztalyid;
                orarend.nap = dto.nap;
                orarend.ora = dto.ora;
                orarend.tantargy_id =tantargyId;
                orarend.tanar_id = tanarId;

                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }

            await Task.CompletedTask;
        }
        #endregion

        #region Delete TimeTable
        public async Task DeleteTimeTable(int orarend_id)
        {
            if (!_context.Orarendek.Any(x => x.orarend_id == orarend_id))
                throw new KeyNotFoundException("Nincs ilyen órarend!");

            using (var trx = _context.Database.BeginTransaction())
            {
                var orarend = _context.Orarendek
                    .First(x => x.orarend_id == orarend_id);

                _context.Orarendek.Remove(orarend);

                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }

            await Task.CompletedTask;
        }
        #endregion

        #region TimeTable Listing
        public Dictionary<DayOfWeek, List<TimeTableItemDto>> GetTimeTable(int osztaly_id)
        {
          
            var result = 
            _context.Orarendek
                .Include(x => x.Tantargy)
                .Include(x => x.Tanar)
                .Where(x => x.osztaly_id == osztaly_id)
                .Select(x => new
                {
                    x.nap,
                    x.ora,
                    tantargyNev = x.Tantargy.tantargy_nev,
                    tanarNev = x.Tanar.tanar_nev
                })
                .GroupBy(x => x.nap)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.ora)
                          .Select(x => new TimeTableItemDto
                          {
                              ora = x.ora,
                              tantargyNev = x.tantargyNev,
                              tanarNev = x.tanarNev
                          })
                          .ToList()
                );


            return result;
        }

        public Dictionary<DayOfWeek, List<TeacherTimeTabelDto>> GetTeacherTimeTable(int tanar_id)
        {
            var result = _context.Orarendek
                .Include(x => x.Tantargy)
                .Include(x => x.Osztaly)
                .Where(x => x.tanar_id == tanar_id)
                .Select(x => new
                {
                    x.nap,
                    x.ora,
                    tantargyNev = x.Tantargy.tantargy_nev,
                    osztalyNev = x.Osztaly.osztaly_nev
                })
                .GroupBy(x => x.nap)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.ora)
                          .Select(x => new TeacherTimeTabelDto
                          {
                              ora = x.ora,
                              tantargyNev = x.tantargyNev,
                              osztalyNev = x.osztalyNev
                          }).ToList()
                );

            return result;
        }
        #endregion
    }
}
