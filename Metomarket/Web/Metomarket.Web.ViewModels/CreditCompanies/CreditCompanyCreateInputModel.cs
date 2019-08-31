using System;
using System.ComponentModel.DataAnnotations;

using Metomarket.Common;
using Metomarket.Web.Infrastructure.Attributes;

namespace Metomarket.Web.ViewModels.CreditCompanies
{
    public class CreditCompanyCreateInputModel
    {
        private const int NameMinLength = 3;
        private const int NameMaxLength = GlobalConstants.CreditCompanyNameMaxLength;
        private const int YearAfter = 1899;
        private const string ActiveSinceDisplayName = "Active Since";
        private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

        public CreditCompanyCreateInputModel()
        {
            this.ActiveSincce = DateTime.Now.AddDays(-1);
        }

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        [IsAfterYear(YearAfter)]
        [IsBeforeCurrentDate]
        [Display(Name = ActiveSinceDisplayName)]
        public DateTime ActiveSincce { get; set; }
    }
}