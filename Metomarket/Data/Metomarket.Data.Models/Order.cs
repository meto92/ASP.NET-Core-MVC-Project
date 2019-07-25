using System;

using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class Order : BaseDeletableModel<string>
    {
        public Order()
        {
            this.IssuedOn = DateTime.UtcNow;
        }

        public DateTime IssuedOn { get; set; }

        public int Quantity { get; set; }

        public bool IsCompleted { get; set; }

        public string ProductId { get; set; }

        public string IssuerId { get; set; }

        public Product Product { get; set; }

        public ApplicationUser Issuer { get; set; }
    }
}