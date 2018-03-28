using System.Collections.Generic;

namespace TP.SF.Common.Model
{
    public class WishList
    {
        public string Name { get; set; }
        public List<WishListItem> Items { get; set; }
        public int Expiration { get; set; }
    }
}