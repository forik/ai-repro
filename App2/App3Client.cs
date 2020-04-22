using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App2
{
    public class App3Client
    {
        private readonly HttpClient _client;

        public App3Client(HttpClient client)
        {
            _client = client;
        }

        public async Task SendAsync(string message)
        {
            await _client.PostAsync("/ping", new StringContent(message, Encoding.UTF8, "application/json"));
        }
    }
}