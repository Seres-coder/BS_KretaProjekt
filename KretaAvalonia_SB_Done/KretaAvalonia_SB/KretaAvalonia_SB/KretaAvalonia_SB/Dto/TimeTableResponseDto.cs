using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.Dto
{
    public class TimeTableResponseDto
    {
        public List<TimeTableDto> Monday { get; set; } = new();
        public List<TimeTableDto> Tuesday { get; set; } = new();
        public List<TimeTableDto> Wednesday { get; set; } = new();
        public List<TimeTableDto> Thursday { get; set; } = new();
        public List<TimeTableDto> Friday { get; set; } = new();
    }
}
