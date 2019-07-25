using System.ComponentModel.DataAnnotations;

using Metomarket.Common;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductEditModel
    {
        private const int NameMinLength = GlobalConstants.ProductNameMinLength;
        private const int NameMaxLength = GlobalConstants.ProductNameMaxLength;
        private const string PriceMinValue = "0.01";
        private const string PriceMaxValue = "1000000";
        private const int ImageUrlMaxLength = GlobalConstants.ProductImageUrlMaxLength;
        private const string ImageUrlDisplayName = "Image URL";
        private const string InStockDisplayName = "Current Quantity";
        private const int InStockMaxValue = 1000;
        private const string QuantityToAddDisplayName = "Quantity to Add";

        public string Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        public string Type { get; set; }

        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        [DataType(DataType.Url)]
        [StringLength(ImageUrlMaxLength)]
        [Display(Name = ImageUrlDisplayName)]
        public string ImageUrl { get; set; }

        [Range(0, InStockMaxValue)]
        [Display(Name = InStockDisplayName)]
        public int InStock { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = QuantityToAddDisplayName)]
        public int QuantityToAdd { get; set; }
    }
}