using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using KretaAv.Dto;

namespace KretaAv.Model
{
    public class OrarendModel
    {
        public ApiSession _session;

        public OrarendModel(ApiSession session)
        {
            _session = session;
        }

        public async Task<int> GetOsztalyId()
        {
            var student = await _session.Client.GetFromJsonAsync<StudentDto>(
                $"api/Data/getmydata?user_id={_session.Userid}");
            if (student == null)
                throw new InvalidOperationException("Nem található a diák adata.");
            return student.osztaly_id;
        }

     
        public async Task<Dictionary<int, List<TimeTableItemDto>>> GetOrarend(int osztaly_id)
        {
            var result = await _session.Client.GetFromJsonAsync<Dictionary<int, List<TimeTableItemDto>>>(
                $"api/TimeTable/gettimetable?osztaly_id={osztaly_id}");
            return result ?? new Dictionary<int, List<TimeTableItemDto>>();
        }
    }
}
