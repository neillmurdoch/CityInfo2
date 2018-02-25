using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];       // "admin@mycompany.com";    // Take from config file, rather than hard coded.
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];   // "noreply@mycompany.com";

        public void Send(string subject, string message)
        {
            // Send mail - output to debug window
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with CloudMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}