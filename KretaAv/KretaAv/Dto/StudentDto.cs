using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAv.Dto
{
    internal class StudentDto
    {
        public int diak_id { get; set; }
        public int user_id { get; set; }
        public int osztaly_id { get; set; }
        public string diak_nev { get; set; }
        public string emailcim { get; set; }
        public string lakcim { get; set; }
        public string szuloneve { get; set; }
        public DateTime szuletesi_datum { get; set; }
    }
}
