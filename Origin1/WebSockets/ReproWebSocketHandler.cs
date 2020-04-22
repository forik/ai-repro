using System.Threading.Tasks;

namespace Origin1.WebSockets
{
    public class ReproWebSocketHandler
    {
        private readonly App2Client _client;

        public ReproWebSocketHandler(App2Client client)
        {
            _client = client;
        }

        public async Task HandleAsync(string message)
            => await _client.SendAsync(message);
    }
}