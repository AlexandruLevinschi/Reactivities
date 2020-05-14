using FluentValidation;

namespace Reactivities.Application.Validators
{
    public static class ValidationExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Password must not be empty")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("^[a-zA-Z0-9]").WithMessage("Password must contain non alphanumeric");

            return options;
        }
    }
}
