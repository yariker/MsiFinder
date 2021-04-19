using System;

namespace MsiFinder.ViewModel
{
    public record ValidationResult
    {
        private ValidationResult(ValidationStatus status, string errorMessage = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Valid { get; } = new(ValidationStatus.Succeeded);

        public ValidationStatus Status { get; }

        public string ErrorMessage { get; }

        public static ValidationResult Invalid(string errorMessage) =>
            new(ValidationStatus.Failed, errorMessage ?? throw new ArgumentNullException(nameof(errorMessage)));
    }
}