using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Ticari / VIP Minivan araçları temsil eder. Kapasite ve kurumsal sigorta farkı uygular.
    /// </summary>
    public class CommercialVehicle : Vehicle
    {
        private int _passengerOrLoadCapacity;

        public CommercialVehicle(int id, string plateNumber, string brand, string model, int year, decimal dailyRate, int capacity = 8)
            : base(id, plateNumber, brand, model, year, dailyRate)
        {
            PassengerOrLoadCapacity = capacity;
        }

        public int PassengerOrLoadCapacity
        {
            get => _passengerOrLoadCapacity;
            set
            {
                if (value <= 0 || value > 50)
                    throw new RentalValidationException("Kapasite 1 ile 50 arasında olmalıdır.");
                _passengerOrLoadCapacity = value;
            }
        }

        public override string VehicleType => "Ticari / VIP";

        public override string SpecificDetails => $"Kapasite: {PassengerOrLoadCapacity} Kişi/Birim";

        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new RentalValidationException("Kiralama süresi en az 1 gün olmalıdır.");

            // Ticari araçlarda günlük 150 TL kurumsal sigorta ve lojistik hizmet bedeli
            decimal baseTotal = DailyRate * days;
            decimal commercialFee = 150m * days;
            return baseTotal + commercialFee;
        }

        public override string GetPricingPolicyDescription()
        {
            return "Ticari / VIP: Günlük 150 TL kurumsal sigorta bedeli eklenir.";
        }
    }
}
