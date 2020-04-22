using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace App2
{
    public class App3Client
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public App3Client(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task SendAsync(string message)
        {
            var uri = new Uri(_config["Dependency"] + "/ping");
            
            await _client.PostAsync(uri, new StringContent(message, Encoding.UTF8, "application/json"));
        }
    }
}