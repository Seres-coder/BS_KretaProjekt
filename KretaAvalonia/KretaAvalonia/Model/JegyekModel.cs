using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using KretaAvalonia.Dto;

namespace KretaAvalonia.Model
{
    public class JegyekModel
    {
        public ApiSession _session;
        private List<GradeListDto> grades = new List<GradeListDto>();


        public JegyekModel(ApiSession session)
        {
            _session = session;
            GetJegyek(_session.Userid);
            grades.Clear
        }
        public async Task GetJegyek(int id)
        {
            grades.Clear();
            var result = await _session.Client.GetFromJsonAsync<List<GradeListDto>>($"api/Grade/allgrade?id={id}&tanar_id=0");
            
            foreach (GradeListDto item in result)
            {
                grades.Add(item);
            }
        }
        public List<GradeListDto> GetGrades()
        {
            return grades;
        }

    }
}
