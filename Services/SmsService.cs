using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CarWashStation.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }

    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public TwilioSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromPhoneNumber"];

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
            {
                Console.WriteLine("Twilio credentials are not configured. Skipping SMS.");
                return Task.CompletedTask;
            }

            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(fromNumber),
                body: message
            );
        }
    }
}
