using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Origin1
{
    public class App2Client
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public App2Client(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task SendAsync(string message)
        {
            var uri = new Uri(_config["Dependency"] + "/ping");
            
            var resp = await _client.PostAsync(uri, new StringContent(message, Encoding.UTF8, "application/json"));
            Console.WriteLine(await resp.Content.ReadAsStringAsync());
        }
    }
}