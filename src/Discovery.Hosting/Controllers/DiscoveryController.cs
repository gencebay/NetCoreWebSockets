using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using NetCoreStack.WebSockets;
using NetStandard.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discovery.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class DiscoveryController : Controller
    {
        private readonly IConnectionManager _connectionManager;

        public DiscoveryController(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        [HttpPost(nameof(SendAsync))]
        public async Task<IActionResult> SendAsync([FromBody]SimpleCacheItem model)
        {
            var echo = $"Echo from server '{model.Id}' '{model.Name}' - {DateTime.Now}";
            var obj = new { message = echo };
            var webSocketContext = new WebSocketMessageContext { Command = WebSocketCommands.DataSend, Value = obj };
            await _connectionManager.BroadcastAsync(webSocketContext);
            return Ok();
        }

        [HttpGet(nameof(CacheReady))]
        public async Task CacheReady()
        {
            var webSocketContext = new WebSocketMessageContext {
                Command = WebSocketCommands.DataSend,
                Header = new RouteValueDictionary(new { CacheReady = Globals.CacheReady }),
                Value = new Context
                {
                    Keys = new string[] { nameof(SimpleCacheItem) }
                }};

            await _connectionManager.BroadcastAsync(webSocketContext);
        }

        [HttpGet(nameof(GetConnections))]
        public IActionResult GetConnections()
        {
            var connections = _connectionManager.Connections
                .Select(x => new { ConnectionId = x.Value.ConnectionId, ConnectorName = x.Value.ConnectorName });

            return Json(connections);
        }
    }
}
