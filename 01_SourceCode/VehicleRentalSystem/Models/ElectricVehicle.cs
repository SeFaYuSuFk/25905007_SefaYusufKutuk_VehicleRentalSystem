using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Elektrikli araçları temsil eder. Çevreci araç teşviki kapsamında %5 indirim uygular.
    /// </summary>
    public class ElectricVehicle : Vehicle
    {
        private int _batteryCapacityKwh;

        public ElectricVehicle(int id, string plateNumber, string brand, string model, int year, decimal dailyRate, int batteryCapacityKwh = 75)
            : base(id, plateNumber, brand, model, year, dailyRate)
        {
            BatteryCapacityKwh = batteryCapacityKwh;
        }

        public int BatteryCapacityKwh
        {
            get => _batteryCapacityKwh;
            set
            {
                if (value <= 0 || value > 300)
                    throw new RentalValidationException("Batarya kapasitesi 1 ile 300 kWh arasında olmalıdır.");
                _batteryCapacityKwh = value;
            }
        }

        public override string VehicleType => "Elektrikli";

        public override string SpecificDetails => $"Batarya: {BatteryCapacityKwh} kWh";

        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new RentalValidationException("Kiralama süresi en az 1 gün olmalıdır.");

            // Elektrikli araçlarda %5 çevreci teşvik indirimi
            return DailyRate * days * 0.95m;
        }

        public override string GetPricingPolicyDescription()
        {
            return "Elektrikli Araç: %5 sıfır emisyon (çevreci) indirimi uygulanır.";
        }
    }
}
