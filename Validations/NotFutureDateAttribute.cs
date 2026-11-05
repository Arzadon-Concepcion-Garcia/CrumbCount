
using System;
using System.ComponentModel.DataAnnotations;

namespace CrumbCount2.Validation
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is not DateTime date || date == default)
            {
                return new ValidationResult(
                    "Please select a valid waste date.");
            }

            if (date.Date > DateTime.Today)
            {
                return new ValidationResult(
                    "Waste date cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
