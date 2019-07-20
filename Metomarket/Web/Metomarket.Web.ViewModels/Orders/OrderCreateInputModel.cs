using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.ViewModels.Orders
{
    public class OrderCreateInputModel
    {
        private const int QuantityMaxValue = 25;

        public string ProductId { get; set; }

        [Range(1, QuantityMaxValue)]
        public int Quantity { get; set; }
    }
}