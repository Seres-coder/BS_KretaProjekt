using KretaAvalonia_SB.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.Model
{
    public class TimeTableModel
    {
        private readonly HttpClient _client;

        public TimeTableModel(HttpClient client)
        {
            _client = client;
        }

        public async Task<TimeTableResponseDto?> GetTimeTable(int classId)
        {
            var response = await _client.GetAsync($"api/TimeTable/gettimetable?osztaly_id={classId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<TimeTableResponseDto>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
