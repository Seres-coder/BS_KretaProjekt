using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.Dto
{
    public class StudentDto
    {
        [JsonPropertyName("diak_nev")]
        public string? diaknev { get; set; }

        [JsonPropertyName("diak_id")]
        public int diakid { get; set; }

        [JsonPropertyName("user_id")]
        public int userid { get; set; }

        public string? emailcim { get; set; }
        public string? lakcim { get; set; }

        [JsonPropertyName("szuletesi_datum")]
        public DateTime? szuletesidatum { get; set; }

        public string? szuloneve { get; set; }

        [JsonPropertyName("osztaly_id")]
        public int? osztalyid { get; set; }
    }
}
