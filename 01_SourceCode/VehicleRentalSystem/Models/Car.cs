using VehicleRentalSystem.Exceptions;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Binek otomobilleri temsil eder. 7 gün ve üzeri kiralamalarda %10 uzun dönem indirimi uygular.
    /// </summary>
    public class Car : Vehicle
    {
        private string _transmissionType = "Otomatik";

        public Car(int id, string plateNumber, string brand, string model, int year, decimal dailyRate, string transmissionType = "Otomatik")
            : base(id, plateNumber, brand, model, year, dailyRate)
        {
            TransmissionType = transmissionType;
        }

        public string TransmissionType
        {
            get => _transmissionType;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new RentalValidationException("Vites tipi boş bırakılamaz.");
                _transmissionType = value.Trim();
            }
        }

        public override string VehicleType => "Otomobil";

        public override string SpecificDetails => $"Vites: {TransmissionType}";

        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new RentalValidationException("Kiralama süresi en az 1 gün olmalıdır.");

            decimal baseTotal = DailyRate * days;
            // 7 gün ve üzeri binek kiralamalarda %10 uzun dönem indirimi
            if (days >= 7)
                return baseTotal * 0.90m;

            return baseTotal;
        }

        public override string GetPricingPolicyDescription()
        {
            return "Binek Otomobil: 7+ gün kiralamalarda %10 uzun dönem indirimi.";
        }
    }
}
