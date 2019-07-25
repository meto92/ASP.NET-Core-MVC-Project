using System.Collections.Generic;

using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class ProductType : BaseModel<string>
    {
        public ProductType()
        {
            this.Products = new HashSet<Product>();
        }

        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}