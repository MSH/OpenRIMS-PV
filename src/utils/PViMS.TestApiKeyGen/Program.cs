using System;
using System.Net.Http;

namespace PViMS.TestApiKeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            HmacClientHandler hmacClientHandler = new HmacClientHandler();
            HttpClient client = HttpClientFactory.Create(hmacClientHandler);

            var url = $"{args[0]}/accounts/pingauth";
            Console.WriteLine($"URL to be pinged: {url}");
            var response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
                Console.WriteLine("Success");
            else
                Console.WriteLine(response.ReasonPhrase);

            Console.ReadLine();
        }
    }
}
