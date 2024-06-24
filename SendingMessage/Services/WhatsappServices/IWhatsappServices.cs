namespace SendingMessage.Services.WhatsappServices
{
    public interface IWhatsappServices
    {
         Task<string> SendingMessage(string phoneNumberId, string recipient, string message, string accessToken);

    }
}
