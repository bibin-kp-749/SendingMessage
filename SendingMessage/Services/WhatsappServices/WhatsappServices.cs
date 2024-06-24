using System.Text.Json;

namespace SendingMessage.Services.WhatsappServices
{
    public class WhatsappServices: IWhatsappServices
    {
        private readonly HttpClient _httpClient;
        public WhatsappServices(HttpClient httpClient)
        {
            this._httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v19.0/");
        }
        public async Task<string> SendingMessage(string phoneNumberId,string recipient,string message,string accessToken)
        {
            var requestUri = $"{phoneNumberId}/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = recipient,
                //type = "template",
                //type = "template",
                //template = new
                //{
                //    name = "hello_world",
                //    language = new
                //    {
                //        code = "en_US"
                //    }
                //}
                type = "text",
                text = new { body = message }
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
