using System.Security.Cryptography.X509Certificates;
using System.Text;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

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
        //Megváltoztatja a felhasználó jelszavát, az újat hashelve menti el
        public async Task ChangePassword(int userId, string ujjelszo)
        {
            if (string.IsNullOrWhiteSpace(ujjelszo))
                throw new ArgumentException("Az új jelszó nem lehet üres.", nameof(ujjelszo));

            var trx = _context.Database.BeginTransaction();
            {
                var user = _context.Users.Where(x => x.user_id == userId).First().jelszo = HashPassword(ujjelszo);
                await _context.SaveChangesAsync();
               await trx.CommitAsync();
            }
        }
        //SHA-256 algoritmussal hasheli a megadott jelszót, Base64 formátumban adja vissza
        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        #endregion
        #region Registration(At first you can only registrate as a student,the admin can upgrade to being teacher)
        //Regisztrál egy új felhasználót; alapértelmezetten diák szerepkörrel, ha a név már foglalt, kivételt dob
        public async Task Registration(string name, string password, string role = "Diak")
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A felhasználónév nem lehet üres.", nameof(name));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A jelszó nem lehet üres.", nameof(password));

            if (await _context.Users.AnyAsync(x => x.belepesnev == name))
                throw new InvalidOperationException("mar letezik");

            var user = new User
            {
                belepesnev = name,
                jelszo = HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);

            if (role == "Diak")
                _context.Diakok.Add(new Diak { User = user }); // <-- EF intézi az FK-t

            await _context.SaveChangesAsync();
        }
        #endregion
        #region Login checker
        //Ellenőrzi a belépési adatokat, sikeres egyezés esetén visszaadja a user adatait, különben null-t
        public UserDto? ValidateUser(string name, string password)
        {
            string hashpass = HashPassword(password);
            var user = _context.Users.Where(x => x.belepesnev == name);
            return user.Where(x => x.jelszo == hashpass).Select(x => new UserDto
            {
                _belepesnev = x.belepesnev,
                _user_id=x.user_id,
                _Role = x.Role,

            }).FirstOrDefault();
        }
        #endregion
        #region Role update
        //Diákból tanárrá lépteti elő a felhasználót; törli a diák rekordot és létrehozza a tanárit
        public async Task PromoteToTanar(int userId, string tantargy)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "Érvénytelen userId.");

            if (string.IsNullOrWhiteSpace(tantargy))
                throw new ArgumentException("A tantárgy nem lehet üres.", nameof(tantargy));


            await using var trx = await _context.Database.BeginTransactionAsync();

            var diak = await _context.Diakok.FirstAsync(x => x.user_id == userId);

            var tantargyid = await _context.Tantargyok
                .Where(x => x.tantargy_nev == tantargy)
                .Select(x => x.tantargy_id)
                .FirstAsync();

            _context.Diakok.Remove(diak);

            _context.Tanarok.Add(new Tanar
            {
                user_id = userId,
                tantargy_id = tantargyid
            });

            var user = await _context.Users.FirstAsync(x => x.user_id == userId);
            user.Role = "Tanar";

            await _context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        #endregion
    }
}
