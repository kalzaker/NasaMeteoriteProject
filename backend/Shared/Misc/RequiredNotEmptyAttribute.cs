using System.ComponentModel.DataAnnotations;

namespace Shared.Misc
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredNotEmptyAttribute : ValidationAttribute
    {
        public RequiredNotEmptyAttribute() 
        {
            ErrorMessage = "{0} is required and cannot be empty.";
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if(value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
        }
    }
}
