using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using TP.SF.Common.Contracts.Actors;
using TP.SF.Common.Model;
using TP.SF.FirstServices.Web.Common;

namespace TP.SF.FirstServices.Web.Controllers
{
    [Route("api/user/wishlist")]
    public class WishListController : Controller
    {
        [HttpPost("")]
        public async Task<IActionResult> CreateWishList([FromBody] WishList wishlist)
        {
            var actor = ActorProxy.Create<IWishlistActor>(new ActorId(wishlist.Name));
            await actor.CreateWishlist(wishlist);
            foreach (var item in wishlist.Items)
            {
                await actor.AddItemToWishlist(item);
            }
            return NoContent();
        }

        [HttpGet("")]
        public async Task<IActionResult> GetWishList()
        {
            ServiceUriBuilder builder = new ServiceUriBuilder("WishlistActorService");
            Uri serviceName = builder.ToUri();
            IList<WishList> results = new List<WishList>();
            FabricClient client = new FabricClient();
            ServicePartitionList partitions = await client.QueryManager.GetPartitionListAsync(serviceName);
            List<ActorInformation> activeActors = new List<ActorInformation>();
            foreach (var partition in partitions)
            {
                long minKey = (partition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var actorServiceProxy = ActorServiceProxy.Create(builder.ToUri(), minKey);
                ContinuationToken continuationToken = null;
                do
                {
                    PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, CancellationToken.None);
                    activeActors.AddRange(page.Items.Where(x => x.IsActive));
                    continuationToken = page.ContinuationToken;
                }
                while (continuationToken != null);
            }
            foreach (var info in activeActors.Distinct())
            {
                var actor = ActorProxy.Create<IWishlistActor>(info.ActorId);
                var result = await actor.GetWishList();
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return Ok(results);
        }
    }
}