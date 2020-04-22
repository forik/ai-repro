using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Origin1.WebSockets
{
    public class ReproWebSocketMiddleware
    {
        private readonly ILogger<ReproWebSocketHandler> _logger;
        private readonly RequestDelegate _next;

        public ReproWebSocketMiddleware(
            ILogger<ReproWebSocketHandler> logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }
        
        public async Task Invoke(HttpContext context, ReproWebSocketHandler handler)
        {
            try
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    await _next.Invoke(context);
                    return;
                }

                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
                await Receive(socket, handler, context.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        
        
        private async Task Receive(WebSocket socket, ReproWebSocketHandler handler,
            CancellationToken contextRequestAborted)    //, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                string serializedMessage = null;
                WebSocketReceiveResult result = null;
                byte[] bytes = ArrayPool<byte>.Shared.Rent(16 * 1024);
                try
                {
                    var buffer = new ArraySegment<byte>(bytes);

                    using MemoryStream ms = new MemoryStream();
                    do
                    {
                        try
                        {
                            result = await socket.ReceiveAsync(buffer, contextRequestAborted).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException e)
                        {
                            _logger.LogWarning(e, "Request aborted");
                            return;
                        }

                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using var reader = new StreamReader(ms, Encoding.UTF8);
                    serializedMessage = await reader.ReadToEndAsync().ConfigureAwait(false);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(bytes);
                }
                
                var acitvity = new Activity("Test");
                acitvity.Start();
                
                Console.WriteLine("Activity: {0}", acitvity.Id);

                await handler.HandleAsync(serializedMessage);

                acitvity.Stop();
            }
        }
    }
}