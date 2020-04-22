﻿using System;
using System.Buffers;
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
        private readonly ReproWebSocketHandler _handler;
        private readonly RequestDelegate _next;

        public ReproWebSocketMiddleware(
            ILogger<ReproWebSocketHandler> logger,
            ReproWebSocketHandler handler,
            RequestDelegate next)
        {
            _logger = logger;
            _handler = handler;
            _next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    await _next.Invoke(context);
                    return;
                }

                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
                await Receive(socket, context.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        
        
        private async Task Receive(WebSocket socket, CancellationToken contextRequestAborted)    //, Action<WebSocketReceiveResult, string> handleMessage)
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

                await _handler.HandleAsync(serializedMessage);
            }
        }
    }
}