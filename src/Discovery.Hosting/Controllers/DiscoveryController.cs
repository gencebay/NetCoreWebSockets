using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using NetCoreStack.WebSockets;
using NetStandard.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discovery.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class DiscoveryController : Controller
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMemoryCache _memoryCache;

        public DiscoveryController(IConnectionManager connectionManager,
            IMemoryCache memoryCache)
        {
            _connectionManager = connectionManager;
            _memoryCache = memoryCache;
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

        [HttpPost(nameof(BroadcastBinaryAsync))]
        public async Task<IActionResult> BroadcastBinaryAsync([FromBody]SimpleCacheItem model)
        {
            var cacheItemList = _memoryCache.Get<List<SimpleCacheItem>>(nameof(SimpleCacheItem));
            
            return Ok();
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
