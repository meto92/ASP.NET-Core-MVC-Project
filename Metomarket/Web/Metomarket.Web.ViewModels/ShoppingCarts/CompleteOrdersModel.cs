using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.ViewModels.ShoppingCarts
{
    public class CompleteOrdersModel : IValidatableObject
    {
        private const string OrdersCountDisplayName = "Orders Count";
        private const int MinMonths = 3;
        private const int MaxMonths = 24;
        private const int MinMonthsStep = 3;
        private const int YearMonthsCount = 12;
        private const string PeriodInMonthsDisplayName = "Choose Period";
        private const string CreditCompanyIdDisplayName = "Credit Company";
        private const string CreditCardNumberDisplayName = "Credit Card Number";
        private const string InvalidPeriod = "Invalid period.";

        public decimal Total { get; set; }

        [Display(Name = OrdersCountDisplayName)]
        public int OrdersCount { get; set; }

        [Range(MinMonths, MaxMonths)]
        [Display(Name = PeriodInMonthsDisplayName)]
        public int PeriodInMonths { get; set; }

        [Required]
        [Display(Name = CreditCompanyIdDisplayName)]
        public string CreditCompanyId { get; set; }

        [Required]
        [CreditCard]
        [Display(Name = CreditCardNumberDisplayName)]
        public string CreditCardNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.PeriodInMonths % MinMonthsStep != 0
                || (this.PeriodInMonths >= YearMonthsCount && this.PeriodInMonths % YearMonthsCount != 0))
            {
                yield return new ValidationResult(InvalidPeriod);
            }

            yield return ValidationResult.Success;
        }
    }
}