using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Origin1
{
    public class App2Client
    {
        private readonly HttpClient _client;

        public App2Client(HttpClient client)
        {
            _client = client;
        }

        public async Task SendAsync(string message)
        {
            var resp = await _client.PostAsync("/ping", new StringContent(message, Encoding.UTF8, "application/json"));
            Console.WriteLine(await resp.Content.ReadAsStringAsync());
        }
    }
}