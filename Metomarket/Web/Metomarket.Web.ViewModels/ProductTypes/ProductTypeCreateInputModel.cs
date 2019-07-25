using System.ComponentModel.DataAnnotations;

using Metomarket.Common;

namespace Metomarket.Web.ViewModels.ProductTypes
{
    public class ProductTypeCreateInputModel
    {
        private const int NameMinLength = 2;
        private const int NameMaxLength = GlobalConstants.ProductTypeNameMaxLength;

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }
    }
}