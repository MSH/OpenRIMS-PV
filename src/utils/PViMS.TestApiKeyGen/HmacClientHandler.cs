using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PViMS.TestApiKeyGen
{
    internal class HmacClientHandler : DelegatingHandler
    {
        private string _apiKey = "bYdvkxBxek5LwTtDLPKqAN4FSV7MfpNr";
        private string _secretKey = "Q345SE2AJeBXgWUD7J8P6pRMB5MqCVdn";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            var requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower()); ;
            var requestHttpMethod = request.Method.Method;

            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            var nonce = Guid.NewGuid().ToString("N");

            var signatureRawData = $"{_apiKey}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}";
            var secretKeyBytes = Convert.FromBase64String(_secretKey);
            var signature = Encoding.UTF8.GetBytes(signatureRawData.Trim());

            using (HMACSHA512 hmac = new HMACSHA512(secretKeyBytes))
            {
                var hashedBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(hashedBytes);
                var apiKeyForServer = $"{_apiKey}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}";

                Console.WriteLine($"API key used: {_apiKey}");
                Console.WriteLine($"Secret key used for encoding signature: {_secretKey}");
                Console.WriteLine($"Hashed signature generated: {requestSignatureBase64String}");
                Console.WriteLine($"Nonce: {nonce}");
                Console.WriteLine($"Timestamp: {requestTimeStamp}");
                Console.WriteLine($"API key generated: {apiKeyForServer}");

                request.Headers.Add("X-Api-Key", apiKeyForServer);
            }

            response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
