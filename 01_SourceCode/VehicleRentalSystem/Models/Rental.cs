using System;
using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Bir müşterinin belirli bir aracı belirli tarihler arasında kiralamasını temsil eder.
    /// </summary>
    public class Rental
    {
        private int _id;
        private Customer _customer = null!;
        private Vehicle _vehicle = null!;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _totalDays;
        private decimal _totalPrice;
        private bool _hasFullInsurance;
        private RentalStatus _status;

        public const decimal DailyInsuranceFee = 250m;

        public Rental(int id, Customer customer, Vehicle vehicle, DateTime startDate, DateTime endDate, bool hasFullInsurance = false)
        {
            Id = id;
            Customer = customer ?? throw new RentalValidationException("Müşteri seçilmelidir.");
            Vehicle = vehicle ?? throw new RentalValidationException("Araç seçilmelidir.");

            if (endDate.Date < startDate.Date)
                throw new RentalValidationException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

            _startDate = startDate.Date;
            _endDate = endDate.Date;

            // Aynı gün iade dahil en az 1 gün sayılır
            int diffDays = (_endDate - _startDate).Days;
            _totalDays = diffDays == 0 ? 1 : diffDays;

            _hasFullInsurance = hasFullInsurance;

            // Polimorfik fiyat hesaplama (araç türüne göre farklı hesaplanır)
            decimal vehiclePrice = _vehicle.CalculateRentalPrice(_totalDays);
            decimal insuranceCost = _hasFullInsurance ? (_totalDays * DailyInsuranceFee) : 0m;
            _totalPrice = vehiclePrice + insuranceCost;

            _status = RentalStatus.Active;
        }

        public int Id
        {
            get => _id;
            private set
            {
                if (value <= 0)
                    throw new RentalValidationException("Kiralama ID sıfırdan büyük olmalıdır.");
                _id = value;
            }
        }

        public Customer Customer
        {
            get => _customer;
            private set => _customer = value;
        }

        public Vehicle Vehicle
        {
            get => _vehicle;
            private set => _vehicle = value;
        }

        public DateTime StartDate => _startDate;
        public DateTime EndDate => _endDate;
        public int TotalDays => _totalDays;
        public decimal TotalPrice => _totalPrice;
        public bool HasFullInsurance => _hasFullInsurance;

        public RentalStatus Status
        {
            get => _status;
            private set => _status = value;
        }

        public string GetStatusDisplay()
        {
            return _status == RentalStatus.Active ? "Aktif" : "Tamamlandı";
        }

        /// <summary>
        /// Kiralama kaydını tamamlandı durumuna getirir ve aracı müsait olarak işaretler.
        /// </summary>
        public void CompleteReturn()
        {
            if (_status == RentalStatus.Completed)
                throw new RentalValidationException("Bu kiralama kaydı zaten tamamlanmış.");

            _status = RentalStatus.Completed;
            _vehicle.MarkAsReturned();
        }
    }
}
