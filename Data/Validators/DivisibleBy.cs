using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Validators
{
    public class DivisibleByAttribute : ValidationAttribute
    {
        private readonly int _divisor;

        public DivisibleByAttribute(int divisor)
        {
            _divisor = divisor;
        }

        public override bool IsValid(object? value)
        {
            if (value is int intValue)
            {
                return intValue > 0 && intValue % _divisor == 0;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be divisible by {_divisor}.";
        }
    }
}
