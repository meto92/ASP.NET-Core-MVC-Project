using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.ViewModels.ProductTypes
{
    public class ProductTypeCreateInputModel
    {
        private const int NameMinLength = 2;
        private const int NameMaxLength = 20;

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }
    }
}