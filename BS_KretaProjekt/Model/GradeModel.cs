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
        //Új jegyet ad hozzá egy diáknak, a tanár neve és a diák neve alapján
        public async Task AddNewGrade(GradeAdd dto)
        {
            if(string.IsNullOrWhiteSpace(dto.tanar_nev) || string.IsNullOrWhiteSpace(dto.tantargy_nev) || string.IsNullOrWhiteSpace(dto.diak_nev) || dto.ertek == 0 )
                throw new InvalidOperationException("Nincs minden adat megadva");

            using var trx = await _context.Database.BeginTransactionAsync();
            var tanarId = await _context.Tanarok.Where(x => x.tanar_nev == dto.tanar_nev).Select(x => x.tanar_id).FirstAsync();
            var tantargyId = await _context.Tantargyok.Where(x => x.tantargy_nev == dto.tantargy_nev).Select(x => x.tantargy_id).FirstAsync();
            var diakId = await _context.Diakok.Where(x => x.diak_nev == dto.diak_nev).Select(x => x.diak_id).FirstAsync();



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



        #endregion
        #region -Grade Modify
        //Módosítja egy meglévő jegy értékét és módosítási dátumát
        public async Task GradeModify(GradeModify dto)
        {
            if (dto.ertek == 0 || dto.updatedatum == DateTimeOffset.MinValue)
                throw new InvalidOperationException("Nincs minden adat megadva");

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
        //Törli a megadott azonosítójú jegyet az adatbázisból
        public async Task DeleteGrade(int id)
        {
            if(!_context.Jegyek.Any(x => x.jegy_id == id))
                throw new KeyNotFoundException("Nincs ilyen jegy");

            using var trx = _context.Database.BeginTransaction();
            _context.Remove(_context.Jegyek.First(x => x.jegy_id == id));
            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        #endregion
        #region -Grade Listing
        //Visszaadja egy diák vagy tanár jegyeit; ha mindkettő meg van adva, kivételt dob
        public IEnumerable<GradeListDto> AllGrades(int id = 0, int tanar_id = 0)
        {
            if (id != 0 && tanar_id != 0)
                throw new InvalidOperationException("Nem lehet egyszerre diák és tanár id alapján keresni");

            if (tanar_id != 0)
            {
                return _context.Jegyek
                    .Include(x => x.Tanar)
                    .Include(x => x.Tantargy)
                    .Where(x => x.tanar_id == tanar_id)
                    .Select(x => new GradeListDto
                    {
                        jegy_id = x.jegy_id,
                        diak_id = x.diak_id,
                        datum = x.datum,
                        ertek = x.ertek,
                        updatedatum = x.updatedatum,
                        tanar_id = x.tanar_id,
                        tantargy_id = x.tantargy_id,
                        tantargyNev = x.Tantargy.tantargy_nev,
                        tanarNev = x.Tanar.tanar_nev
                    })
                    .ToList();
            }
            else if (id != 0)
            {
                return _context.Jegyek
                    .Include(x => x.Tanar)
                    .Include(x => x.Tantargy)
                    .Where(x => x.diak_id == id)
                    .Select(x => new GradeListDto
                    {
                        jegy_id = x.jegy_id,
                        diak_id = x.diak_id,
                        datum = x.datum,
                        ertek = x.ertek,
                        updatedatum = x.updatedatum,
                        tanar_id = x.tanar_id,
                        tantargy_id = x.tantargy_id,
                        tantargyNev = x.Tantargy.tantargy_nev,
                        tanarNev = x.Tanar.tanar_nev
                    })
                    .ToList();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        #endregion
    }
}
