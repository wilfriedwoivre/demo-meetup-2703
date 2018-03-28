using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Threading.Tasks;
using Demo2703.Domain;
using Demo2703.Interfaces.Services;
using Demo2703.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Demo2703.Web.Controllers
{
    [Route("api/session")]
    public class SessionController : Controller
    {
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] Session session)
        {
            Guid id = Guid.NewGuid();
            session.Id = id;
            var servicePartitioney = new ServicePartitionKey(id.GetHashCode()); 
            ServiceUriBuilder builder = new ServiceUriBuilder("Demo2703.SessionManager");

            ISessionManager manager = ServiceProxy.Create<ISessionManager>(builder.ToUri(), servicePartitioney);
            await manager.CreateSessionAsync(session);

            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSession([FromRoute]Guid id)
        {
            try
            {
                var servicePartitionKey = new ServicePartitionKey(id.GetHashCode());
                ServiceUriBuilder builder = new ServiceUriBuilder("Demo2703.SessionManager");

                ISessionManager manager = ServiceProxy.Create<ISessionManager>(builder.ToUri(), servicePartitionKey);

                var session = await manager.GetSessionAsync(id);

                return Ok(session);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetSessions()
        {
            ServiceUriBuilder builder = new ServiceUriBuilder("Demo2703.SessionManager");
            Uri serviceName = builder.ToUri();

            List<Session> results = new List<Session>();
            FabricClient client = new FabricClient();
            ServicePartitionList partitions = await client.QueryManager.GetPartitionListAsync(serviceName);

            foreach (var partition in partitions)
            {
                long minKey = (partition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                ISessionManager service = ServiceProxy.Create<ISessionManager>(builder.ToUri(), new ServicePartitionKey(minKey));

                IEnumerable<Session> subResult = await service.ListSessionAsync();
                if (subResult != null)
                {
                    results.AddRange(subResult);
                }

            }
            return Ok(results);
        }



    }
}
