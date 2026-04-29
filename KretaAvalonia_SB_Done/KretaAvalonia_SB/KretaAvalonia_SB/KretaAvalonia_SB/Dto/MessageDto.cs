using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string tartalom { get; set; }
        public string cim { get; set; }

        public string kuldoname { get; set; }/*küldő user ->admin vagy tanar*/

        public DateTimeOffset kuldesidopontja { get; set; }
    }
}
