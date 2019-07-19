using System;
using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.Infrastructure.Attributes
{
    public class IsAfterYearAttribute : ValidationAttribute
    {
        private const string InvalidDataType = "Invalid data type. Expected DateTime.";
        private const string DateTooEarly = "Date is too early. Minimum year: {0}";

        private readonly int year;

        public IsAfterYearAttribute(int year)
        {
            this.year = year;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.GetType() != typeof(DateTime))
            {
                return new ValidationResult(InvalidDataType);
            }

            DateTime dateTimeValue = (DateTime)value;

            if (dateTimeValue.Year <= this.year)
            {
                return new ValidationResult(string.Format(
                    DateTooEarly,
                    this.year + 1));
            }

            return ValidationResult.Success;
        }
    }
}