using NetCoreStack.WebSockets;
using System.Threading.Tasks;

namespace Discovery.Hosting
{
    public class ServerWebSocketCommandInvocator : IServerWebSocketCommandInvocator
    {
        private readonly IConnectionManager _connectionManager;
        public ServerWebSocketCommandInvocator(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            var connection = context.GetConnectionId();
            await _connectionManager.BroadcastAsync(context);
        }
    }
}