using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MeetSlot.Validation
{
    // Gjenbrukbar validering: én dato må være større enn en annen dato i samme DTO.
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value as DateTime?;

            var comparisonProperty = validationContext.ObjectType.GetProperty(
                _comparisonProperty,
                BindingFlags.Public | BindingFlags.Instance);

            var comparisonValue = comparisonProperty?.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (!currentValue.HasValue || !comparisonValue.HasValue)
            {
                // Lar [Required]-validering håndtere manglende felter.
                return ValidationResult.Success;
            }

            if (currentValue.Value <= comparisonValue.Value)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.MemberName} må være etter {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
