using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public class SMTPMailService : ISMTPMailService
    {
        private readonly string _smtpHost;
        private readonly int _port;
        private readonly bool _useSSL;
        private readonly string _mailboxUserName;
        private readonly string _mailboxPassword;
        private readonly string _mailboxAddress;

        public SMTPMailService(string smtpHost, int port, bool useSSL, string mailboxUserName, string mailboxPassword, string mailboxAddress)
        {
            if (string.IsNullOrEmpty(smtpHost))
            {
                throw new System.ArgumentException($"'{nameof(smtpHost)}' cannot be null or empty.", nameof(smtpHost));
            }

            if (string.IsNullOrEmpty(mailboxUserName))
            {
                throw new System.ArgumentException($"'{nameof(mailboxUserName)}' cannot be null or empty.", nameof(mailboxUserName));
            }

            if (string.IsNullOrEmpty(mailboxPassword))
            {
                throw new System.ArgumentException($"'{nameof(mailboxPassword)}' cannot be null or empty.", nameof(mailboxPassword));
            }

            if (string.IsNullOrEmpty(mailboxAddress))
            {
                throw new System.ArgumentException($"'{nameof(mailboxAddress)}' cannot be null or empty.", nameof(mailboxAddress));
            }

            _smtpHost = smtpHost;
            _port = port;
            _useSSL = useSSL;
            _mailboxUserName = mailboxUserName;
            _mailboxPassword = mailboxPassword;
            _mailboxAddress = mailboxAddress;
        }

        public async Task SendEmailAsync(string subject, string body, List<MailboxAddress> destinationAddresses)
        {
            ValidateSendDetails(subject, body, destinationAddresses);

            var mailMessage = new MimeMessage();

            mailMessage.From.Add(new MailboxAddress(_mailboxAddress, _mailboxAddress));
            mailMessage.To.AddRange(destinationAddresses);
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("html")
            {
                Text = body
            };

            using (var smtpClient = new SmtpClient())
            {
                //smtpClient.ServerCertificateValidationCallback = MySslCertificateValidationCallback;
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                //await smtpClient.ConnectAsync(_smtpHost, _port, _useSSL);
                //await smtpClient.AuthenticateAsync(_mailboxUserName, _mailboxPassword);
                //await smtpClient.SendAsync(mailMessage);
                //await smtpClient.DisconnectAsync(true);
            }
        }

        private void ValidateSendDetails(string subject, string body, List<MailboxAddress> destinationAddresses)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new System.ArgumentException($"{nameof(subject)} cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new System.ArgumentException($"{nameof(body)} cannot be null or empty.");
            }

            if (destinationAddresses == null)
            {
                throw new System.ArgumentNullException($"{nameof(destinationAddresses)} cannot be null.");
            }

            if (destinationAddresses.Count == 0)
            {
                throw new System.ArgumentException($"{nameof(destinationAddresses)} cannot be empty.");
            }
        }

        private bool MySslCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If there are no errors, then everything went smoothly.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Note: MailKit will always pass the host name string as the `sender` argument.
            var host = (string)sender;

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
            {
                // This means that the remote certificate is unavailable. Notify the user and return false.
                //throw new DomainException($"The SSL certificate was not available for {host}");
                return false;
            }

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                // This means that the server's SSL certificate did not match the host name that we are trying to connect to.
                var certificate2 = certificate as X509Certificate2;
                var cn = certificate2 != null ? certificate2.GetNameInfo(X509NameType.SimpleName, false) : certificate.Subject;

                //throw new DomainException($"The Common Name for the SSL certificate did not match {host}. Instead, it was {cn}.");
                return false;
            }

            // The only other errors left are chain errors.
            //Console.WriteLine("The SSL certificate for the server could not be validated for the following reasons:");

            // The first element's certificate will be the server's SSL certificate (and will match the `certificate` argument)
            // while the last element in the chain will typically either be the Root Certificate Authority's certificate -or- it
            // will be a non-authoritative self-signed certificate that the server admin created. 
            //foreach (var element in chain.ChainElements)
            //{
            //    // Each element in the chain will have its own status list. If the status list is empty, it means that the
            //    // certificate itself did not contain any errors.
            //    if (element.ChainElementStatus.Length == 0)
            //        continue;

            //    Console.WriteLine("\u2022 {0}", element.Certificate.Subject);
            //    foreach (var error in element.ChainElementStatus)
            //    {
            //        // `error.StatusInformation` contains a human-readable error string while `error.Status` is the corresponding enum value.
            //        Console.WriteLine("\t\u2022 {0}", error.StatusInformation);
            //    }
            //}

            return false;
        }
    }
}