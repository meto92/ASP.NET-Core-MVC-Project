using System;
using System.Collections.Generic;

using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class Contract : BaseModel<string>
    {
        public Contract()
        {
            this.IssuedOn = DateTime.UtcNow;
            this.Orders = new HashSet<Order>();
        }

        public DateTime IssuedOn { get; set; }

        public DateTime ActiveUntil { get; set; }

        public decimal PricePerMonth { get; set; }

        public string CustomerId { get; set; }

        public string CompanyId { get; set; }

        public ApplicationUser Customer { get; set; }

        public CreditCompany Company { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}