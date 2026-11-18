using System;

namespace VehicleRentalSystem.Exceptions
{
    /// <summary>
    /// İş kuralları veya girdi doğrulama ihlallerinde fırlatılan özel istisna sınıfı.
    /// </summary>
    public class RentalValidationException : Exception
    {
        public RentalValidationException(string message) : base(message)
        {
        }

        public RentalValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
