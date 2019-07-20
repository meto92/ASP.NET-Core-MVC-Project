using System.ComponentModel.DataAnnotations;

using Metomarket.Web.ViewModels.Orders;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductCreateOrderViewModel
    {
        private const string NameDisplayName = "Product Name";

        public string Id { get; set; }

        [Display(Name = NameDisplayName)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public OrderCreateInputModel Input { get; set; }
    }
}