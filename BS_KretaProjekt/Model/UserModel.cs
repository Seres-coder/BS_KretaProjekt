using System.Text;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;

namespace BS_KretaProjekt.Model
{
    public class UserModel
    {
        private readonly KretaDbContext _context;
        public UserModel (KretaDbContext context)
        {
            _context = context;
        }
        #region Password Change

        public void ChangePassword(int userId, string ujjelszo)
        {
            var trx = _context.Database.BeginTransaction();
            {
                var user = _context.Users.Where(x => x.user_id == userId).First().jelszo = HashPassword(ujjelszo);
                _context.SaveChanges();
                trx.Commit();
            }
        }
        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        #endregion
        #region Registration(At first you can only registrate as a student,the admin can upgrade to being teacher)
        public void Registration(string name, string password, string role = "Diak")
        {
            if (_context.Users.Any(x => x.belepesnev == name))
            {
                throw new InvalidOperationException("mar letezik");
            }
            var trx = _context.Database.BeginTransaction();
            {
                _context.Users.Add(new User { belepesnev = name, jelszo = HashPassword(password), Role = role });
                int id = _context.Users.Last().user_id;/*reménykedjünk, hogy működik--> todo*/
                if (role == "Diak")
                {
                    _context.Diakok.Add(new Diak
                    {
                        diak_id = id,
                    });
                }
                _context.SaveChanges();
                trx.Commit();
            }
        }
        #endregion
        #region Login checker
        public UserDto? ValidateUser(string name, string password, string role = "Diak")
        {
            string hashpass = HashPassword(password);
            var user = _context.Users.Where(x => x.belepesnev == name);
            return user.Where(x => x.jelszo == hashpass).Select(x => new UserDto
            {
                _belepesnev = x.belepesnev,
                _jelszo = x.jelszo,
                _Role = x.Role,

            }).FirstOrDefault();
        }
        #endregion
        #region Role update
        public void PromoteToTanar(int userId, string tantargy)
        {
            var trx = _context.Database.BeginTransaction();
            {
                var user = _context.Diakok.Where(x => x.diak_id == userId).First();
                var tantargyid = _context.Tantargyok.Where(x => x.tantargy_nev == tantargy).First().tantargy_id;
                _context.Diakok.Remove(user);
                _context.Tanarok.Add(new Tanar
                {
                    user_id = userId,
                    tanar_id = userId,
                    Role = "Tanar",
                    tantargy_id = tantargyid,

                });
                _context.SaveChanges();
                trx.Commit();
            }
        }
        #endregion
    }
}
