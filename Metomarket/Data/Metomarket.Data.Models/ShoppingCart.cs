using System.Collections.Generic;

using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class ShoppingCart : BaseModel<string>
    {
        public ShoppingCart()
        {
            this.Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; }

        public ApplicationUser Customer { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}