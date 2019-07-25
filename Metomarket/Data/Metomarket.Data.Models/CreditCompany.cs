using System;
using System.Collections.Generic;

using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class CreditCompany : BaseModel<string>
    {
        public CreditCompany()
        {
            this.Contracts = new HashSet<Contract>();
        }

        public string Name { get; set; }

        public DateTime ActiveSince { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }
}