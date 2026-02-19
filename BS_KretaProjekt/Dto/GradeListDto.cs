using BS_KretaProjekt.Persistence;

namespace BS_KretaProjekt.Dto
{
    public class GradeListDto
    {
        public int jegy_id { get; set; }
        public DateTimeOffset datum { get; set; }
        public DateTimeOffset updatedatum { get; set; }
        public int ertek { get; set; }
        public int tantargy_id { get; set; }
        public int tanar_id { get; set; }
    }
}
