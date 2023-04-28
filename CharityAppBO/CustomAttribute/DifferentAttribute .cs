using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DifferentAttribute : ValidationAttribute
{
    private readonly string otherProperty;

    public DifferentAttribute(string otherProperty)
    {
        this.otherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var otherPropertyValue = validationContext.ObjectType.GetProperty(otherProperty)?.GetValue(validationContext.ObjectInstance);
        if (otherPropertyValue != null && value != null && Equals(value, otherPropertyValue))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
