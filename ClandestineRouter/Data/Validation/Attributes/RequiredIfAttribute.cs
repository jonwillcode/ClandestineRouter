using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Validation.Attributes;

public class RequiredIfAttribute(string dependentProperty, object? targetValue = null) : ValidationAttribute
{
    private readonly string _dependentProperty = dependentProperty;
    private readonly object? _targetValue = targetValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dependentProperty = validationContext.ObjectType.GetProperty(_dependentProperty) ?? throw new ArgumentException($"Property with name {_dependentProperty} not found when attempting RequiredIfAttribute validation.");
        var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance);

        // If dependent property has a value (or matches target value if specified)
        bool shouldBeRequired = _targetValue == null
            ? dependentValue != null && !string.IsNullOrWhiteSpace(dependentValue?.ToString())
            : Equals(dependentValue, _targetValue);

        if (shouldBeRequired && (value == null || string.IsNullOrWhiteSpace(value?.ToString())))
        {
            // FUN FACT: If the second variable is not provided, `memberNames`, it will cause <ValidationMessage> to not
            // display anything. However, it will still display in <ValidationSummary />.
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required", [validationContext?.MemberName?? ""]);
        }

        return ValidationResult.Success;
    }
}