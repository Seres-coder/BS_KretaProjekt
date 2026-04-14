using KretaAvalonia_SB.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KretaAvalonia_SB.Model
{
    public class GradeModel
    {
        private readonly HttpClient _client;

        public GradeModel(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<GradeDto>?> GetMyGrade(int studentId)
        {
            var response = await _client.GetAsync($"api/Grade/allgrade?id={studentId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<GradeDto>>();
        }

     
    }
}
