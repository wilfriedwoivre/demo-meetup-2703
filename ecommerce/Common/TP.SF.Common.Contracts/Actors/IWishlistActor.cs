using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using TP.SF.Common.Model;

namespace TP.SF.Common.Contracts.Actors
{
    public interface IWishlistActor : IActor
    {
        Task CreateWishlist(WishList wishlist);

        Task AddItemToWishlist(WishListItem item);

        Task<WishList> GetWishList();
    }
}
