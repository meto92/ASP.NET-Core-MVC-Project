using System.ComponentModel.DataAnnotations;

using Metomarket.Common;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductCreateInputModel
    {
        private const int NameMinLength = GlobalConstants.ProductNameMinLength;
        private const int NameMaxLength = GlobalConstants.ProductNameMaxLength;
        private const int InStockMaxValue = 1000;
        private const string PriceMinValue = "0.01";
        private const string PriceMaxValue = "1000000";
        private const string InStockDisplayName = "Initial quantity";
        private const int ImageUrlMaxLength = GlobalConstants.ProductImageUrlMaxLength;
        private const string ImageUrlDisplayName = "Image URL";
        private const string TypeIdDisplayName = "Type";
        private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        [Range(0, InStockMaxValue)]
        [Display(Name = InStockDisplayName)]
        public int InStock { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [StringLength(ImageUrlMaxLength)]
        [Display(Name = ImageUrlDisplayName)]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = TypeIdDisplayName)]
        public string TypeId { get; set; }
    }
}