namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Aracın güncel kiralama durumunu temsil eder.
    /// </summary>
    public enum VehicleStatus
    {
        Available, // Müsait
        Rented     // Kirada
    }

    /// <summary>
    /// Kiralama kaydının durumunu temsil eder.
    /// </summary>
    public enum RentalStatus
    {
        Active,    // Aktif Kiralama
        Completed  // Tamamlandı (İade Edildi)
    }
}
