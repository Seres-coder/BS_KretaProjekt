using System.Formats.Asn1;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Model
{
    public class DataModel
    {
        private readonly KretaDbContext _context;
        public DataModel(KretaDbContext context)
        {
            _context = context;
        }

        public async Task<StudentDto> GetMyData(int user_id) 
        {
           var items= await _context.Diakok.Where(x => x.user_id == user_id).Select(x => new StudentDto
            {
                diak_nev = x.diak_nev,
                diak_id = x.diak_id,
                user_id = x.user_id,
                emailcim = x.emailcim,
                lakcim = x.lakcim,
                szuletesi_datum = (DateTime)x.szuletesi_datum,
                szuloneve = x.szuloneve,
                osztaly_id = (int)x.osztaly_id
            }).FirstAsync();
            return items;

        }

        public async Task<TeacherDto> GetMyTeacherData(int user_id)
        {
            var items= await _context.Tanarok.Where(x=>x.user_id==user_id).Select(x=> new TeacherDto { tanar_id = x.tanar_id,szak=x.szak,tanar_nev=x.tanar_nev}).FirstAsync();
            return items;
        }

        public async Task<IEnumerable<StudentDto>> GetDiak()
        {

            var items = await _context.Diakok
                .Select(x => new StudentDto
                {

                    diak_nev = x.diak_nev ?? "",
                    user_id = x.user_id,
                    osztaly_id = x.osztaly_id ?? 0,
                    lakcim = x.lakcim ?? "",
                    szuloneve = x.szuloneve ?? "",
                    emailcim = x.emailcim ?? "",
                    szuletesi_datum = x.szuletesi_datum ?? DateTime.MinValue
                })
                .ToListAsync();
            if (items.Count == 0)
                throw new InvalidOperationException("nincs diak");
            return items;
        }
        public async Task<IEnumerable<TeacherDto>> GetTeacher()
        {
            var items = await _context.Tanarok.Select(x => new TeacherDto
            {
                tanar_id = x.tanar_id,
                tanar_nev = x.tanar_nev,
                szak = x.szak,
            }).ToListAsync();
            if (items.Count == 0)
                throw new InvalidOperationException("nincs tanar");
            return items;

        }

        public async Task ModifyStudentData(StudentDto dto)
        {
            var diak = await _context.Diakok.SingleOrDefaultAsync(x => x.diak_id == dto.diak_id);
            if (diak is null)
                throw new InvalidOperationException();
            if (string.IsNullOrWhiteSpace(dto.diak_nev) || string.IsNullOrWhiteSpace(dto.lakcim) || string.IsNullOrWhiteSpace(dto.szuloneve) || string.IsNullOrWhiteSpace(dto.emailcim) || dto.szuletesi_datum == DateTime.MinValue)
                throw new InvalidOperationException("Nincs minden adat megadva");
            if (!_context.Osztalyok.Any(x => x.osztaly_id == dto.osztaly_id))
                throw new KeyNotFoundException("Nincs ilyen diak");


            await using var trx = await _context.Database.BeginTransactionAsync();
            diak.diak_id = dto.diak_id;
            diak.diak_nev = dto.diak_nev;
            diak.osztaly_id = dto.osztaly_id;
            diak.lakcim = dto.lakcim;
            diak.szuloneve = dto.szuloneve;
            diak.emailcim = dto.emailcim;
            diak.szuletesi_datum = dto.szuletesi_datum;

            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }



        public async Task ModifyTeacherData(TeacherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.tanar_nev) || string.IsNullOrWhiteSpace(dto.szak))
                throw new InvalidOperationException("Nincs minden adat megadva");
            if (!_context.Tanarok.Any(x => x.tanar_id == dto.tanar_id))
                throw new KeyNotFoundException("Nincs ilyen tanar");

            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Tanarok.Where(x => x.tanar_id == dto.tanar_id).First().tanar_nev = dto.tanar_nev;
                _context.Tanarok.Where(x => x.tanar_id == dto.tanar_id).First().szak = dto.szak;
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            await Task.CompletedTask;
        }
        public async Task DeleteStudentData(int id)
        {
            var diak = await _context.Diakok.SingleOrDefaultAsync(x => x.diak_id == id);
            if (diak is null)
                throw new InvalidOperationException("nincs ilyen diak");
            var user = await _context.Users.SingleOrDefaultAsync(x => x.user_id == diak.user_id);
            var jegyek = _context.Jegyek.Where(x => x.diak_id == id);
            var uzenetek = _context.Uzenetek.Where(x => x.fogado_id == id);
            await using var trx = await _context.Database.BeginTransactionAsync();
            _context.Jegyek.RemoveRange(jegyek);
            _context.Uzenetek.RemoveRange(uzenetek);
            _context.Diakok.Remove(diak);
            if (user != null)
                _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        public async Task DeleteTeacherData(int id)
        {
            var tanar = await _context.Tanarok
                .SingleOrDefaultAsync(x => x.tanar_id == id);
            if (tanar is null)
                throw new InvalidOperationException("nincs ilyen tanar");
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.user_id == tanar.user_id);
            var uzenetek = _context.Uzenetek.Where(x => x.fogado_id == id);
            await using var trx = await _context.Database.BeginTransactionAsync();
            _context.Jegyek.RemoveRange(_context.Jegyek.Where(x => x.tanar_id == id));
            _context.Orarendek.RemoveRange(_context.Orarendek.Where(x => x.tanar_id == id));
            _context.Uzenetek.RemoveRange(uzenetek);
            _context.Tanarok.Remove(tanar);
            if (user != null)
                _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
    }
}
