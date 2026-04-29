using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace KretaAvalonia_SB.Dto
{


    public class LoginResDto
    {
        public int id { get; set; }       
        public string? name { get; set; }
        public string? role { get; set; }
    }
}
