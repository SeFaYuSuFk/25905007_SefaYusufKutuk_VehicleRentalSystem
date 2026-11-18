using System;
using System.Collections.Generic;
using System.Linq;
using VehicleRentalSystem.Exceptions;
using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Services
{
    /// <summary>
    /// Araç, müşteri ve kiralama koleksiyonlarını yöneten merkezi iş mantığı sınıfı.
    /// </summary>
    public class RentalManager
    {
        private readonly List<Vehicle> _vehicles = new();
        private readonly List<Customer> _customers = new();
        private readonly List<Rental> _rentals = new();

        private int _nextVehicleId = 1;
        private int _nextCustomerId = 1;
        private int _nextRentalId = 1;

        /// <summary>
        /// Veri koleksiyonlarında değişiklik olduğunda UI bileşenlerini bilgilendiren olay (Delegate/Event).
        /// </summary>
        public event Action? DataChanged;

        public RentalManager(bool loadSampleData = true)
        {
            if (loadSampleData)
                SeedSampleData();
        }

        public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();
        public IReadOnlyList<Customer> Customers => _customers.AsReadOnly();
        public IReadOnlyList<Rental> Rentals => _rentals.AsReadOnly();

        // Dashboard dinamik istatistikleri
        public int TotalVehicleCount => _vehicles.Count;
        public int AvailableVehicleCount => _vehicles.Count(v => v.Status == VehicleStatus.Available);
        public int RentedVehicleCount => _vehicles.Count(v => v.Status == VehicleStatus.Rented);
        public int TotalCustomerCount => _customers.Count;
        public int ActiveRentalCount => _rentals.Count(r => r.Status == RentalStatus.Active);
        public decimal TotalRevenue => _rentals.Sum(r => r.TotalPrice);

        public int GetNextVehicleId() => _nextVehicleId;
        public int GetNextCustomerId() => _nextCustomerId;

        private void OnDataChanged()
        {
            DataChanged?.Invoke();
        }

        #region Araç İşlemleri

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new RentalValidationException("Araç bilgisi geçersiz.");

            bool plateExists = _vehicles.Any(v =>
                string.Equals(v.PlateNumber, vehicle.PlateNumber, StringComparison.OrdinalIgnoreCase));

            if (plateExists)
                throw new RentalValidationException($"'{vehicle.PlateNumber}' plakalı araç sistemde zaten kayıtlıdır!");

            _vehicles.Add(vehicle);
            if (vehicle.Id >= _nextVehicleId)
                _nextVehicleId = vehicle.Id + 1;

            OnDataChanged();
        }

        public void UpdateVehicle(int id, string plateNumber, string brand, string model, int year, decimal dailyRate, string? extraDetail = null)
        {
            Vehicle? existing = _vehicles.FirstOrDefault(v => v.Id == id);
            if (existing == null)
                throw new RentalValidationException("Güncellenecek araç bulunamadı.");

            string normalizedPlate = plateNumber.Trim().ToUpperInvariant();
            bool duplicatePlate = _vehicles.Any(v =>
                v.Id != id && string.Equals(v.PlateNumber, normalizedPlate, StringComparison.OrdinalIgnoreCase));

            if (duplicatePlate)
                throw new RentalValidationException($"'{normalizedPlate}' plakası başka bir araca aittir!");

            existing.PlateNumber = plateNumber;
            existing.Brand = brand;
            existing.Model = model;
            existing.Year = year;
            existing.DailyRate = dailyRate;

            // Türe özgü özellik güncellemesi
            if (!string.IsNullOrWhiteSpace(extraDetail))
            {
                switch (existing)
                {
                    case Car car:
                        car.TransmissionType = extraDetail;
                        break;
                    case Suv suv:
                        suv.IsAllWheelDrive = extraDetail.Contains("4x4", StringComparison.OrdinalIgnoreCase);
                        break;
                    case ElectricVehicle ev when int.TryParse(extraDetail, out int kwh):
                        ev.BatteryCapacityKwh = kwh;
                        break;
                    case CommercialVehicle cv when int.TryParse(extraDetail, out int cap):
                        cv.PassengerOrLoadCapacity = cap;
                        break;
                }
            }

            OnDataChanged();
        }

        public void DeleteVehicle(int id)
        {
            Vehicle? vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
                throw new RentalValidationException("Silinecek araç bulunamadı.");

            if (vehicle.Status == VehicleStatus.Rented)
                throw new RentalValidationException($"'{vehicle.PlateNumber}' plakalı araç şu anda kirada olduğu için silinemez!");

            bool hasRentalHistory = _rentals.Any(r => r.Vehicle.Id == id);
            if (hasRentalHistory)
                throw new RentalValidationException($"'{vehicle.PlateNumber}' plakalı aracın kiralama geçmişi bulunduğu için silinemez!");

            _vehicles.Remove(vehicle);
            OnDataChanged();
        }

        public List<Vehicle> GetAvailableVehicles()
        {
            return _vehicles.Where(v => v.Status == VehicleStatus.Available).ToList();
        }

        #endregion

        #region Müşteri İşlemleri

        public void AddCustomer(Customer customer)
        {
            if (customer == null)
                throw new RentalValidationException("Müşteri bilgisi geçersiz.");

            bool licenseExists = _customers.Any(c =>
                string.Equals(c.LicenseNumber, customer.LicenseNumber, StringComparison.OrdinalIgnoreCase));

            if (licenseExists)
                throw new RentalValidationException($"'{customer.LicenseNumber}' ehliyet numaralı müşteri zaten kayıtlıdır!");

            _customers.Add(customer);
            if (customer.Id >= _nextCustomerId)
                _nextCustomerId = customer.Id + 1;

            OnDataChanged();
        }

        public void UpdateCustomer(int id, string firstName, string lastName, string phone, string licenseNumber)
        {
            Customer? existing = _customers.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                throw new RentalValidationException("Güncellenecek müşteri bulunamadı.");

            string normalizedLicense = licenseNumber.Trim().ToUpperInvariant();
            bool duplicateLicense = _customers.Any(c =>
                c.Id != id && string.Equals(c.LicenseNumber, normalizedLicense, StringComparison.OrdinalIgnoreCase));

            if (duplicateLicense)
                throw new RentalValidationException($"'{normalizedLicense}' ehliyet numarası başka bir müşteriye aittir!");

            existing.FirstName = firstName;
            existing.LastName = lastName;
            existing.Phone = phone;
            existing.LicenseNumber = licenseNumber;

            OnDataChanged();
        }

        public void DeleteCustomer(int id)
        {
            Customer? customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                throw new RentalValidationException("Silinecek müşteri bulunamadı.");

            bool hasActiveRental = _rentals.Any(r => r.Customer.Id == id && r.Status == RentalStatus.Active);
            if (hasActiveRental)
                throw new RentalValidationException($"'{customer.FullName}' adlı müşterinin aktif kiralaması bulunduğu için silinemez!");

            _customers.Remove(customer);
            OnDataChanged();
        }

        #endregion

        #region Kiralama ve İade İşlemleri

        public Rental CreateRental(Customer customer, Vehicle vehicle, DateTime startDate, DateTime endDate, bool hasFullInsurance = false)
        {
            if (customer == null)
                throw new RentalValidationException("Lütfen bir müşteri seçin.");

            if (vehicle == null)
                throw new RentalValidationException("Lütfen müsait bir araç seçin.");

            if (vehicle.Status != VehicleStatus.Available)
                throw new RentalValidationException($"'{vehicle.PlateNumber}' plakalı araç şu anda müsait değildir!");

            Rental rental = new Rental(_nextRentalId, customer, vehicle, startDate, endDate, hasFullInsurance);
            vehicle.MarkAsRented();

            _rentals.Add(rental);
            _nextRentalId++;

            OnDataChanged();
            return rental;
        }

        public List<Rental> GetActiveRentals()
        {
            return _rentals.Where(r => r.Status == RentalStatus.Active).ToList();
        }

        public void ReturnVehicle(int rentalId)
        {
            Rental? rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
                throw new RentalValidationException("Kiralama kaydı bulunamadı.");

            rental.CompleteReturn();
            OnDataChanged();
        }

        #endregion

        #region Örnek Başlangıç Verileri (Seed Data)

        public void SeedSampleData()
        {
            _vehicles.Clear();
            _customers.Clear();
            _rentals.Clear();
            _nextVehicleId = 1;
            _nextCustomerId = 1;
            _nextRentalId = 1;

            // Farklı OOP alt sınıflarından örnek araçlar
            AddVehicle(new Car(_nextVehicleId, "34 SYK 001", "BMW", "320i M Sport", 2024, 3200m, "Otomatik"));
            AddVehicle(new Car(_nextVehicleId, "06 ANK 412", "Renault", "Megane", 2023, 1650m, "Otomatik"));
            AddVehicle(new ElectricVehicle(_nextVehicleId, "34 TOG 107", "TOGG", "T10X V2", 2025, 2900m, 88));
            AddVehicle(new Suv(_nextVehicleId, "35 IZM 777", "Volvo", "XC90 Recharge", 2024, 4800m, true));
            AddVehicle(new CommercialVehicle(_nextVehicleId, "16 BRS 505", "Mercedes-Benz", "Vito Tourer VIP", 2023, 3500m, 9));
            AddVehicle(new ElectricVehicle(_nextVehicleId, "34 TSL 202", "Tesla", "Model Y LR", 2025, 3400m, 75));

            // Örnek müşteriler
            AddCustomer(new Customer(_nextCustomerId, "Ahmet", "Yılmaz", "0532 111 22 33", "B-2021-45891"));
            AddCustomer(new Customer(_nextCustomerId, "Zeynep", "Kaya", "0542 333 44 55", "B-2019-78234"));
            AddCustomer(new Customer(_nextCustomerId, "Mehmet", "Demir", "0555 666 77 88", "B-2022-11904"));
            AddCustomer(new Customer(_nextCustomerId, "Elif", "Şahin", "0533 999 00 11", "B-2020-66321"));

            // Örnek 1 aktif kiralama (Dashboard'da "Kirada" ve "Aktif Kiralama" canlı görünsün diye)
            CreateRental(_customers[0], _vehicles[1], DateTime.Today, DateTime.Today.AddDays(3), true);
        }

        #endregion
    }
}
