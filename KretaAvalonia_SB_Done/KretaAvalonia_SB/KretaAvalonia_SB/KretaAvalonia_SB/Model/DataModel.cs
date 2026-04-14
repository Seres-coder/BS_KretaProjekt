using KretaAvalonia_SB.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KretaAvalonia_SB.Model
{
    public class DataModel
    {

        private readonly HttpClient _client;

        public DataModel(HttpClient client)
        {
            _client = client;
        }


        public async Task<StudentDto?> GetMyStudentData(int userId)
        {
            
            var response = await _client.GetAsync($"api/Data/getmydata?user_id={userId}");
            
            var json = await response.Content.ReadAsStringAsync();
           

            if (!response.IsSuccessStatusCode) return null;
            return System.Text.Json.JsonSerializer.Deserialize<StudentDto>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
