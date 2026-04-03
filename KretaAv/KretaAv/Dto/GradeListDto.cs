using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAv.Dto
{
    public class GradeListDto
    {
        public int jegy_id { get; set; }
        public int diak_id { get; set; }
        public DateTimeOffset datum { get; set; }
        public DateTimeOffset updatedatum { get; set; }
        public int ertek { get; set; }
        public int tantargy_id { get; set; }
        public int tanar_id { get; set; }

        public string tantargyNev { get; set; }
        public string tanarNev { get; set; }
    }
}
