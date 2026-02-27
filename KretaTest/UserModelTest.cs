using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KretaTest
{
    public class UserModelTest
    {
        private readonly UserModel _model;
        private readonly KretaDbContext _context;

        public UserModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new UserModel(_context);
        }

        [Fact]
        public async Task Registration_Correct()
        {
            var belepesnev = "tesztnev";
            var password = "teeszt123";
            var before = await _context.Users.CountAsync();
            await _model.Registration(belepesnev, password);
            var after = await _context.Users.CountAsync();
            Assert.Equal(before + 1, after);
            var createdUser = await _context.Users.SingleOrDefaultAsync(x => x.belepesnev == belepesnev);
            Assert.NotNull(createdUser);
            Assert.Equal(belepesnev, createdUser.belepesnev);
            Assert.False(string.IsNullOrWhiteSpace(createdUser.jelszo));
            Assert.NotEqual(password, createdUser.jelszo);
        }
        [Fact]
        public void Login()
        {
            UserDto User = _model.ValidateUser("admin", "admin123");
            Assert.Equal(User._Role, "Admin");
        }
        [Fact]
        public async Task PromoteTanar()
        {
            int usersBefore = _context.Users.Count();
            int diakBefore = _context.Diakok.Count();
            int tanarBefore = _context.Tanarok.Count();

            int userId = _context.Users.First(x => x.belepesnev == "diak1").user_id;

            await _model.PromoteToTanar(userId, "Matematika");

            Assert.Equal(usersBefore, _context.Users.Count());
            Assert.Equal(diakBefore - 1, _context.Diakok.Count());
            Assert.Equal(tanarBefore + 1, _context.Tanarok.Count());

            var user = _context.Users.First(x => x.user_id == userId);
            Assert.Equal("Tanar", user.Role);

            var tanar = _context.Tanarok.First(t => t.user_id == userId);
            Assert.NotNull(tanar);
        }

        [Fact]
        public async Task ChangePassword()
        {
            var user = await _context.Users.FirstAsync(x => x.belepesnev == "diak1");
            var oldHash = user.jelszo;
            var newPlainPassword = "ujjelszo_123";
            await _model.ChangePassword(user.user_id, newPlainPassword);
            var updated = await _context.Users.FirstAsync(x => x.user_id == user.user_id);
            Assert.NotNull(updated);
            Assert.False(string.IsNullOrWhiteSpace(updated.jelszo));
            Assert.NotEqual(oldHash, updated.jelszo);
            Assert.NotEqual(newPlainPassword, updated.jelszo);
        }

    }
}
