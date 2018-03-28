using System;
using System.Threading;
using System.Threading.Tasks;
using Demo2703.Interfaces.Actors;
using Demo2703.Interfaces.Services;
using Demo2703.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Demo2703.Web.Controllers
{
    [Route("api/game")]
    public class GameController : Controller
    {
        [HttpPost("{sessionId:guid}")]
        public async Task<IActionResult> Create([FromRoute] Guid sessionId)
        {
            var servicePartitionKey = new ServicePartitionKey(sessionId.GetHashCode());
            ServiceUriBuilder builder = new ServiceUriBuilder("Demo2703.SessionManager");

            ISessionManager manager = ServiceProxy.Create<ISessionManager>(builder.ToUri(), servicePartitionKey);

            var session = await manager.GetSessionAsync(sessionId);


            var actor = ActorProxy.Create<IOperationActor>(new ActorId(Guid.NewGuid()));
            var operation = await actor.GenerateOperationAsync((int) session.Difficulty, CancellationToken.None);

            return CreatedAtAction(nameof(ValidResult), new { sessionId = session.Id, operationId = operation.Id }, operation);
        }

        [HttpPost("valid/{sessionId:guid}/{operationId:guid}")]
        public async Task<IActionResult> ValidResult([FromRoute] Guid sessionId, [FromRoute] Guid operationId, [FromBody] long result)
        {
            var actor = ActorProxy.Create<IOperationActor>(new ActorId(operationId));

            var operation = await actor.GetOperationAsync(CancellationToken.None);
            var isValid = await actor.IsValidResult(result, CancellationToken.None);

            var servicePartitionKey = new ServicePartitionKey(sessionId.GetHashCode());
            ServiceUriBuilder builder = new ServiceUriBuilder("Demo2703.SessionManager");

            ISessionManager manager = ServiceProxy.Create<ISessionManager>(builder.ToUri(), servicePartitionKey);

            await manager.AddGameToSessionAsync(sessionId, operation, isValid);
            
            return Ok(isValid);
        }

    }
}