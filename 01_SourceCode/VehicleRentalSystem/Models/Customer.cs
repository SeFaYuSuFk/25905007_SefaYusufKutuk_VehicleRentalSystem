using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Araç kiralayacak müşteri bilgilerini ve doğrulama kurallarını temsil eder.
    /// </summary>
    public class Customer
    {
        private int _id;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _phone = string.Empty;
        private string _licenseNumber = string.Empty;

        public Customer(int id, string firstName, string lastName, string phone, string licenseNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            LicenseNumber = licenseNumber;
        }

        public int Id
        {
            get => _id;
            private set
            {
                if (value <= 0)
                    throw new RentalValidationException("Müşteri ID sıfırdan büyük olmalıdır.");
                _id = value;
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Müşteri adı boş bırakılamaz.");
                _firstName = value.Trim();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Müşteri soyadı boş bırakılamaz.");
                _lastName = value.Trim();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 7)
                    throw new RentalValidationException("Geçerli bir telefon numarası girilmelidir (en az 7 karakter).");
                _phone = value.Trim();
            }
        }

        public string LicenseNumber
        {
            get => _licenseNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Ehliyet numarası boş bırakılamaz.");
                _licenseNumber = value.Trim().ToUpperInvariant();
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} (Ehliyet: {LicenseNumber})";
        }
    }
}
