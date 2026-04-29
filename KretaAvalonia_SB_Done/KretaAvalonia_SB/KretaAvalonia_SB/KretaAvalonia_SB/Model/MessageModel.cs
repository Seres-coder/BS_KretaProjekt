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
    public class MessageModel
    {
        private readonly HttpClient _client;

        public MessageModel(HttpClient client)
        {
            _client = client;
        }
        //Lekéri a diák összes bejövő üzenetét a szervertől a fogadó azonosítója alapján
        public async Task<List<MessageDto>?> GetMyMessage(int studentId)
        {
            var response = await _client.GetAsync($"api/Message/messageklistazasa?fogado_id={studentId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<MessageDto>>();
        }
    }
}
