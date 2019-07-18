using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductCreateInputModel
    {
        private const int NameMinLength = 2;
        private const int NameMaxLength = 20;
        private const int InStockMaxValue = 1000;
        private const string PriceMinValue = "0.01";
        private const string PriceMaxValue = "1000000";
        private const string InStockDisplayName = "Initial quantity";
        private const string ImageUrlDisplayName = "Image URL";
        private const string TypeIdDisplayName = "Type";

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        [Range(0, InStockMaxValue)]
        [Display(Name = InStockDisplayName)]
        public int InStock { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = ImageUrlDisplayName)]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = TypeIdDisplayName)]
        public string TypeId { get; set; }
    }
}