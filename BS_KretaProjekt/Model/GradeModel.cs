using System.Diagnostics;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Model
{
    public class GradeModel
    {
        private readonly KretaDbContext _context;
        public GradeModel(KretaDbContext context)
        {
            _context = context;
        }
        #region Grade Add
        public async Task AddNewGrade(GradeAdd dto)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                var tanarId = _context.Tanarok.First(x => x.tanar_nev == dto.tanar_nev).tanar_id;
                var tantargyId = _context.Tantargyok.First(x => x.tantargy_nev == dto.tantargy_nev).tantargy_id;
                var diakId = _context.Diakok.First(x => x.diak_nev == dto.diak_nev).diak_id;
                _context.Jegyek.Add(new Jegy
                {
                    datum = DateTimeOffset.Now,
                    updatedatum = DateTimeOffset.Now,
                    ertek = dto.ertek,
                    tantargy_id = tantargyId,
                    diak_id = diakId,
                    tanar_id = tanarId
                });
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            await Task.CompletedTask;
        }
        #endregion
        #region -Grade Modify
        public async Task GradeModify(GradeModify dto)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Jegyek.Where(x => x.jegy_id == dto.jegy_id).First().ertek = dto.ertek;
                _context.Jegyek.Where(x => x.jegy_id == dto.jegy_id).First().updatedatum = dto.updatedatum;
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            await Task.CompletedTask;
        }
        #endregion
        #region -Grade Delete
        public async Task DeleteGrade(int id)
        {
            using var trx = _context.Database.BeginTransaction();
            _context.Remove(_context.Jegyek.First(x => x.jegy_id == id));
            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        #endregion
        #region -Grade Listing
        public IEnumerable<GradeListDto> AllGrades(int id = 0, int tanar_id = 0)
        {
            if (tanar_id != 0)
            {
                return _context.Jegyek.Include(x => x.Tanar).Include(x => x.tantargy).Where(x => x.tanar_id == x.tanar_id).Select(x => new GradeListDto
                {
                    jegy_id = x.jegy_id,
                    datum = x.datum,
                    ertek = x.ertek,
                    updatedatum = x.updatedatum,
                    tanar_id = x.tanar_id,
                    tantargy_id = x.tantargy_id
                }).ToList();
            }
            else if (id != 0)
            {


                return _context.Jegyek.Include(x => x.Tanar).Include(x => x.tantargy).Where(x => x.diak_id == id).Select(x => new GradeListDto
                {
                    jegy_id = x.jegy_id,
                    datum = x.datum,
                    ertek = x.ertek,
                    updatedatum = x.updatedatum,
                    tanar_id = x.tanar_id,
                    tantargy_id = x.tantargy_id
                }).ToList();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        #endregion
    }
}
