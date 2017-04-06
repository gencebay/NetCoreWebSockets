using NetCoreStack.WebSockets;
using NetCoreStack.WebSockets.ProxyClient;
using System;
using System.Threading.Tasks;

namespace WebClient.Hosting
{
    public class ProxyWebSocketCommandInvocator : IClientWebSocketCommandInvocator
    {
        public Task InvokeAsync(WebSocketMessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
