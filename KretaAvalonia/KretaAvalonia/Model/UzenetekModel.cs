using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using KretaAvalonia.Dto;

namespace KretaAvalonia.Model
{
    public class UzenetekModel
    {
        public ApiSession _session;

        public UzenetekModel(ApiSession session)
        {
            _session = session;
        }
        public async Task<List<UzenetekDto>> GetUzenetek()
        {
            var result = await _session.Client.GetFromJsonAsync<List<UzenetekDto>>("api/uzenet/messageklistazasa");
            return result;
        }
        public async Task<CreateMessageDto> AddNewMessage(CreateMessageDto dto)
        {
            var response = await _session.Client.PostAsJsonAsync("api/uzenet/messageadd", dto);

            response.EnsureSuccessStatusCode(); // ha kell, hogy dobjon hibát, ha nem 200

            var result = await response.Content.ReadFromJsonAsync<CreateMessageDto>();

            return result;
        }
    }
}
