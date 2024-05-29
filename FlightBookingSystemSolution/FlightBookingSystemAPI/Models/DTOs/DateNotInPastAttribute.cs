using System;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    // Custom validation attribute
    public class DateNotInPastAttribute : ValidationAttribute
    {
        public DateNotInPastAttribute()
        {
            ErrorMessage = "The date cannot be in the past.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime < DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
