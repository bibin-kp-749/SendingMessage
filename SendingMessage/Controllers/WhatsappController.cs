using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendingMessage.Services.WhatsappServices;
using Serilog;
using System.Text.Json;

namespace SendingMessage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly IWhatsappServices _whatsappServices;
        private const string PhoneNumberId = "375798672277169";
        private const string AccessToken = "EAAXNXc6GmwQBO3JTzWEl3KgZCDUSNgHHssNd5741GUtKzmKoXlbirjFVxdSjqaPOPyxnkvyKZB8E3927guZAU4GtAcsaepMBfAZCUNNBsHGkHVYkDZCXk8HHnTQ6UbhiLZBq8UvTkWc7awRXR0vNALZAyWsoZAQmoOqEFkrhB0c7IOqkexE547yM8epJZCJM63uZB36sUcBAADtqIzHSaMu7Gd";
        public WhatsappController(IWhatsappServices whatsappServices)
        {
            this._whatsappServices = whatsappServices;
        }
        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessage([FromForm]string to,[FromForm]string message)
        {
            var result=await _whatsappServices.SendingMessage(PhoneNumberId,to,message,AccessToken);
            return Ok(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
        {
            try
            {
                // Log the received payload
                Log.Information($"Received payload: {payload}");

                // Example of processing the payload
                if (payload.TryGetProperty("entry", out JsonElement entries))
                {
                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (entry.TryGetProperty("changes", out JsonElement changes))
                        {
                            foreach (var change in changes.EnumerateArray())
                            {
                                if (change.TryGetProperty("value", out JsonElement value))
                                {
                                    if (value.TryGetProperty("messages", out JsonElement messages))
                                    {
                                        foreach (var message in messages.EnumerateArray())
                                        {
                                            var from = message.GetProperty("from").GetString();
                                            var text = message.GetProperty("text").GetProperty("body").GetString();

                                            Log.Information($"Received message from {from}: {text}");
                                           // Process the message as needed
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Information($"Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("webhook")]
        public IActionResult VerifyWebhook([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
        {
            // Verify the token
            const string verifyToken = "your_verify_token";
            if (hub_mode == "subscribe" && hub_verify_token == verifyToken)
            {
                return Ok(hub_challenge);
            }

            return Unauthorized();
        }
    }
}
