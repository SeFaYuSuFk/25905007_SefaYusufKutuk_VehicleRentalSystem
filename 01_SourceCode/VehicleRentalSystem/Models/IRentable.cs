namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Kiralanabilir varlıklar için ortak sözleşmeyi tanımlar (Soyutlama - Interface).
    /// </summary>
    public interface IRentable
    {
        /// <summary>
        /// Belirtilen gün sayısı için toplam kiralama ücretini hesaplar.
        /// </summary>
        decimal CalculateRentalPrice(int days);

        /// <summary>
        /// Kiralama işlemi başlatıldığında durumu günceller.
        /// </summary>
        void MarkAsRented();

        /// <summary>
        /// İade işlemi tamamlandığında durumu müsait hale getirir.
        /// </summary>
        void MarkAsReturned();
    }
}
