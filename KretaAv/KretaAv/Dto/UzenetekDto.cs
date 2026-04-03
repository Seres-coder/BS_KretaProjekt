using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia.Dto
{

    public class UzenetekDto
    {
        public int uzenet_id { get; set; }
        public string tartalom { get; set; }
        public string cim { get; set; }
        public int fogado_id { get; set; }/*fogado user ->diak*/
        public int user_id { get; set; }/*küldő user ->admin vagy tanar*/
    }
}

