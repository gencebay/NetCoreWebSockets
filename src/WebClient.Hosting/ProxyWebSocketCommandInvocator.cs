using NetCoreStack.WebSockets;
using NetCoreStack.WebSockets.ProxyClient;
using NetStandard.Contracts;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient.Hosting
{
    public class ProxyWebSocketCommandInvocator : IClientWebSocketCommandInvocator
    {
        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            await Task.CompletedTask;

            if (context.Command == WebSocketCommands.DataSend)
            {
                object cacheReady = null;
                if (context.Header.TryGetValue(nameof(Globals.CacheReady), out cacheReady))
                {
                    Interlocked.Increment(ref ApplicationVariables.CacheReady);
                }
            }

            if (context.MessageType == WebSocketMessageType.Binary)
            {
                // noop
            }
        }
    }
}