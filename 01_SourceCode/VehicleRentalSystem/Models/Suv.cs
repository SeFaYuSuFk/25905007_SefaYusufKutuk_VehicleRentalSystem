using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Arazi / 4x4 SUV araçları temsil eder. Zorlu yol ve arazi kaskosu nedeniyle %15 ek tarife uygular.
    /// </summary>
    public class Suv : Vehicle
    {
        private bool _isAllWheelDrive;

        public Suv(int id, string plateNumber, string brand, string model, int year, decimal dailyRate, bool isAllWheelDrive = true)
            : base(id, plateNumber, brand, model, year, dailyRate)
        {
            IsAllWheelDrive = isAllWheelDrive;
        }

        public bool IsAllWheelDrive
        {
            get => _isAllWheelDrive;
            set => _isAllWheelDrive = value;
        }

        public override string VehicleType => "SUV / 4x4";

        public override string SpecificDetails => IsAllWheelDrive ? "Çekiş: 4x4 AWD" : "Çekiş: 4x2";

        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new RentalValidationException("Kiralama süresi en az 1 gün olmalıdır.");

            // SUV araçlarda arazi kaskosu ve bakım farkı (%15)
            decimal multiplier = IsAllWheelDrive ? 1.15m : 1.08m;
            return DailyRate * days * multiplier;
        }

        public override string GetPricingPolicyDescription()
        {
            return IsAllWheelDrive
                ? "SUV (4x4): %15 arazi kaskosu ve çekiş paketi dahil."
                : "SUV (4x2): %8 ek kasko paketi dahil.";
        }
    }
}
