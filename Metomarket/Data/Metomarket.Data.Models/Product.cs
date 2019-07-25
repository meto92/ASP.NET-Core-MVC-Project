using Metomarket.Data.Common.Models;

namespace Metomarket.Data.Models
{
    public class Product : BaseDeletableModel<string>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int InStock { get; set; }

        public string ImageUrl { get; set; }

        public string TypeId { get; set; }

        public ProductType Type { get; set; }
    }
}