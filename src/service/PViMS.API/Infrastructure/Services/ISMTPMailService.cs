using MimeKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface ISMTPMailService
    {
        Task SendEmailAsync(string subject, string body, List<MailboxAddress> destinationAddresses);
    }
}
