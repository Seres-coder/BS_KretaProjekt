using BS_KretaProjekt.Persistence;

namespace BS_KretaProjekt.Model
{
    public class GradeModel
    {
        private readonly KretaDbContext _context;
        public GradeModel(KretaDbContext context)
        {
            _context = context;
        }

    }
}
