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
        public IEnumerable<StudentDto> GetDiak()
        {
            return _context.Diakok
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
                .ToList();
        }
        public IEnumerable<TeacherDto> GetTeacher()
        {
            return _context.Tanarok.Select(x => new TeacherDto
            {
                tanar_id = x.tanar_id,
                tanar_nev = x.tanar_nev,
                szak = x.szak,
            });
        }
        
           public async Task ModifyStudentData(StudentDto dto)
        {
            var diak = await _context.Diakok.SingleOrDefaultAsync(x => x.diak_id == dto.diak_id);
            if (diak is null)
                throw new InvalidOperationException(); 

            await using var trx = await _context.Database.BeginTransactionAsync();
            diak.diak_id=dto.diak_id;   
            diak.diak_nev = dto.diak_nev;
            diak.osztaly_id = dto.osztaly_id;
            diak.lakcim = dto.lakcim;
            diak.szuloneve = dto.szuloneve;
            diak.emailcim = dto.emailcim;
            diak.szuletesi_datum = dto.szuletesi_datum;

            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        
        public async Task ModifyTeacherData( TeacherDto dto)
        {
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
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Tanarok.Remove(_context.Tanarok.Where(x => x.tanar_id == id).First());
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            await Task.CompletedTask;
        }
    }
}
