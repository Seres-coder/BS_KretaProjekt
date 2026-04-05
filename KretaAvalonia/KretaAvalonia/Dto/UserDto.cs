using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia.Dto
{
    public class UserDto
    {
        public int _user_id { get; set; }
        public string _belepesnev { get; set; }
        public string _jelszo { get; set; }
        public string? _Role { get; set; }
        public string? _osztaly { get; set; }
        public string? _tantargy { get; set; }
    }
}
