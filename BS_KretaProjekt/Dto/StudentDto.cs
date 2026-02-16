using BS_KretaProjekt.Persistence;

namespace BS_KretaProjekt.Dto
{
    public class StudentDto
    {
        public string diak_nev { get; set; }
        public int user_id { get; set; }
        public int osztaly_id { get; set; }
        public string lakcim { get; set; }
        public string szuloneve { get; set; }
        public string emailcim { get; set; }
        public int jegyek { get; set; }
        public DateTime szuletesi_datum { get; set; }

    }
}
