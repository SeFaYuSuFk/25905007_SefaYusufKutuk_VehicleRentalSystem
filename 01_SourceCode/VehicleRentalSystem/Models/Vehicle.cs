using System;
using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Tüm araç türleri için ortak alanları, kapsülleme kurallarını ve soyut sözleşmeyi tanımlayan temel sınıf.
    /// </summary>
    public abstract class Vehicle : IRentable
    {
        private int _id;
        private string _plateNumber = string.Empty;
        private string _brand = string.Empty;
        private string _model = string.Empty;
        private int _year;
        private decimal _dailyRate;
        private VehicleStatus _status;

        protected Vehicle(int id, string plateNumber, string brand, string model, int year, decimal dailyRate)
        {
            Id = id;
            PlateNumber = plateNumber;
            Brand = brand;
            Model = model;
            Year = year;
            DailyRate = dailyRate;
            _status = VehicleStatus.Available;
        }

        public int Id
        {
            get => _id;
            private set
            {
                if (value <= 0)
                    throw new RentalValidationException("Araç ID değeri sıfırdan büyük olmalıdır.");
                _id = value;
            }
        }

        public string PlateNumber
        {
            get => _plateNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Plaka alanı boş bırakılamaz.");
                _plateNumber = value.Trim().ToUpperInvariant();
            }
        }

        public string Brand
        {
            get => _brand;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Marka alanı boş bırakılamaz.");
                _brand = value.Trim();
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Model alanı boş bırakılamaz.");
                _model = value.Trim();
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                int maxYear = DateTime.Now.Year + 1;
                if (value < 1950 || value > maxYear)
                    throw new RentalValidationException($"Üretim yılı 1950 ile {maxYear} arasında olmalıdır.");
                _year = value;
            }
        }

        public decimal DailyRate
        {
            get => _dailyRate;
            set
            {
                if (value <= 0)
                    throw new RentalValidationException("Günlük kira ücreti sıfırdan büyük olmalıdır.");
                _dailyRate = value;
            }
        }

        public VehicleStatus Status
        {
            get => _status;
            protected set => _status = value;
        }

        /// <summary>
        /// Araç türünün kullanıcı arayüzünde gösterilecek adı (Polymorphism).
        /// </summary>
        public abstract string VehicleType { get; }

        /// <summary>
        /// Araç türüne özgü ek detay metni (vites, çekiş, batarya, kapasite vb.).
        /// </summary>
        public abstract string SpecificDetails { get; }

        /// <summary>
        /// Gün sayısına göre araç türüne özel kiralama ücretini hesaplar (Abstract / Polymorphism).
        /// </summary>
        public abstract decimal CalculateRentalPrice(int days);

        /// <summary>
        /// Araç türüne özel fiyatlandırma politikasını açıklar (Virtual / Polymorphism).
        /// </summary>
        public virtual string GetPricingPolicyDescription()
        {
            return "Standart günlük tarife uygulanır.";
        }

        public void MarkAsRented()
        {
            if (_status == VehicleStatus.Rented)
                throw new RentalValidationException($"{PlateNumber} plakalı araç zaten kirada.");
            _status = VehicleStatus.Rented;
        }

        public void MarkAsReturned()
        {
            _status = VehicleStatus.Available;
        }

        public string GetStatusDisplay()
        {
            return _status == VehicleStatus.Available ? "Müsait" : "Kirada";
        }

        public override string ToString()
        {
            return $"{PlateNumber} - {Brand} {Model} ({Year}) [{VehicleType}]";
        }
    }
}
