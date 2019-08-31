using System.ComponentModel.DataAnnotations;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.ProductTypes
{
    public class ProductTypeEditModel : IMapFrom<ProductType>
    {
        private const int NameMinLength = GlobalConstants.ProductTypeNameMinLength;
        private const int NameMaxLength = GlobalConstants.ProductTypeNameMaxLength;
        private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = NameMinLength)]
        public string Name { get; set; }
    }
}