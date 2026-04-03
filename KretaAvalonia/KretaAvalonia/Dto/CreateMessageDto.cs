using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia.Dto
{
    public class CreateMessageDto
    {
        public string tartalom { get; set; }
        public string cim { get; set; }

        public int fogado_id { get; set; }/*fogado usAer ->diak*/

        public int user_id { get; set; }/*küldő user ->admin vagy tanar*/

    }
}
