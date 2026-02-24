using System.Formats.Asn1;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;

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
            return _context.Diakok.Select(x => new StudentDto
            {
                diak_nev = x.diak_nev,
                user_id = x.user_id,
                osztaly_id = x.osztaly_id,
                lakcim = x.lakcim,
                szuloneve = x.szuloneve,
                emailcim = x.emailcim,
                jegyek = x.jegyek,
                szuletesi_datum = x.szuletesi_datum,
            });
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
        public async Task ModifyStudetData(int id, StudentDto dto)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Diakok.Where(x => x.diak_id == id).First().diak_nev = dto.diak_nev;
                _context.Diakok.Where(x => x.diak_id == id).First().osztaly_id = dto.osztaly_id;
                _context.Diakok.Where(x => x.diak_id == id).First().lakcim = dto.lakcim;
                _context.Diakok.Where(x => x.diak_id == id).First().szuloneve = dto.szuloneve;
                _context.Diakok.Where(x => x.diak_id == id).First().emailcim = dto.emailcim;
                _context.Diakok.Where(x => x.diak_id == id).First().jegyek = dto.jegyek;
                _context.Diakok.Where(x => x.diak_id == id).First().szuletesi_datum = dto.szuletesi_datum;
                _context.SaveChanges();
                trx.Commit();
            }
            await Task.CompletedTask;
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
        public async Task AddStudentData(StudentDto dto)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Diakok.Add(new Diak
                {
                    diak_nev = dto.diak_nev,
                    osztaly_id = dto.osztaly_id,
                    lakcim = dto.lakcim,
                    szuloneve = dto.szuloneve,
                    emailcim = dto.emailcim,
                    szuletesi_datum = dto.szuletesi_datum,
                });
            }
            await Task.CompletedTask;
        }
        public async Task AddTeacherData(TeacherDto dto)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Tanarok.Add(new Tanar
                {
                    tanar_nev = dto.tanar_nev,
                    szak = dto.szak,
                });
            }
            await Task.CompletedTask;
        }
        public async Task DeleteStudentData(int id)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Diakok.Remove(_context.Diakok.Where(x => x.diak_id == id).First());
                _context.SaveChanges();
                trx.Commit();
            }
            await Task.CompletedTask;
        }
        public async Task DeleteTeacherData(int id)
        {
            using (var trx = _context.Database.BeginTransaction())
            {
                _context.Tanarok.Remove(_context.Tanarok.Where(x => x.tanar_id == id).First());
                _context.SaveChanges();
                trx.Commit();
            }
            await Task.CompletedTask;
        }
    }
}
