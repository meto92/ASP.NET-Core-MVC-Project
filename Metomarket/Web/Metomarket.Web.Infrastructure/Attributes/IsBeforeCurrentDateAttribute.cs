using System;
using System.ComponentModel.DataAnnotations;

namespace Metomarket.Web.Infrastructure.Attributes
{
    public class IsBeforeCurrentDateAttribute : ValidationAttribute
    {
        private const string InvalidDataType = "Invalid data type. Expected DateTime.";
        private const string DateIsAfterCurrentDate = "Date should not be after current date.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.GetType() != typeof(DateTime))
            {
                return new ValidationResult(InvalidDataType);
            }

            DateTime dateTimeValue = (DateTime)value;

            if (dateTimeValue >= DateTime.Now)
            {
                return new ValidationResult(DateIsAfterCurrentDate);
            }

            return ValidationResult.Success;
        }
    }
}