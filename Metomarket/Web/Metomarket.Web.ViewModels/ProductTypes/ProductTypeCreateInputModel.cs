using System.ComponentModel.DataAnnotations;

using Metomarket.Common;

namespace Metomarket.Web.ViewModels.ProductTypes
{
    public class ProductTypeCreateInputModel
    {
        private const int NameMinLength = GlobalConstants.ProductTypeNameMinLength;
        private const int NameMaxLength = GlobalConstants.ProductTypeNameMaxLength;
        private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = NameMinLength)]
        public string Name { get; set; }
    }
}