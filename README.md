<div align="center">

# 🚗 SYK Fleet — Araç Kiralama Yönetim Sistemi
### *(Vehicle Rental Management System)*

[![C#](https://img.shields.io/badge/Language-C%23%2013-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-10.0%20WinForms-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![OOP](https://img.shields.io/badge/Architecture-OOP%20%26%20SOLID-0D9488?style=for-the-badge)](https://github.com/SeFaYuSuFk/25905007_SefaYusufKutuk_VehicleRentalSystem)
[![Status](https://img.shields.io/badge/Build-Passing-16A34A?style=for-the-badge)](https://github.com/SeFaYuSuFk/25905007_SefaYusufKutuk_VehicleRentalSystem)

**BGY203 / YBS223 — Nesne Tabanlı Programlama Dersi Proje Ödevi**  
Modern Windows Forms arayüzüne sahip, katmanlı ve gerçek polimorfizm uygulayan masaüstü filo & araç kiralama yönetim sistemi.

</div>

---

## 👨‍💻 Geliştirici ve Ders Bilgileri

| Bilgi | Açıklama |
| :--- | :--- |
| **Öğrenci Adı Soyadı** | **Sefa Yusuf Kütük** |
| **Okul Numarası** | **25905007** |
| **Bölüm** | Yazılım Geliştirme |
| **Ders Kodu & Adı** | BGY203 / YBS223 • Nesne Tabanlı Programlama |
| **Öğretim Elemanı** | Dr. Öğr. Üyesi Tohid YOUSEFİ |
| **Geliştirme Ortamı** | C# • .NET 10.0 Windows Forms • Visual Studio / Git |
| **GitHub Repository** | [SeFaYuSuFk/25905007_SefaYusufKutuk_VehicleRentalSystem](https://github.com/SeFaYuSuFk/25905007_SefaYusufKutuk_VehicleRentalSystem) *(Private)* |

---

## 🎯 Projenin Amacı ve Öne Çıkan Farklarımız

Bu proje, standart CRUD işlemlerinin ötesine geçerek **Nesne Tabanlı Programlama (OOP)** ilkelerini gerçek bir iş akışı içinde çalışır halde sunmak amacıyla geliştirilmiştir:

- **4 Farklı Araç Sınıfı ile Gerçek Polimorfizm:** Yalnızca tek bir `Car` sınıfı yerine; `Car` (Binek Otomobil), `Suv` (4x4 Arazi), `ElectricVehicle` (Elektrikli) ve `CommercialVehicle` (Ticari/VIP) sınıfları `CalculateRentalPrice(int days)` metodunu kendi tarife kurallarına göre `override` eder.
- **Event-Driven (Olay Tabanlı) Senkronizasyon:** `RentalManager` içindeki `event Action? DataChanged` delegesi sayesinde herhangi bir modülde yapılan ekleme, silme, kiralama veya iade işlemi Dashboard ve tüm listeleri anında günceller.
- **Modern Dark/Flat Dashboard Tasarımı:** Sol yan menü (Sidebar), renkli dinamik istatistik kartları, canlı arama/filtreleme çubukları ve sağ tarafta anlık güncellenen **Kiralama Ücret Özeti** paneli.
- **Özel İş Kuralı Doğrulamaları:** `RentalValidationException` özel istisna sınıfı ile plaka tekrarı, ehliyet no tekrarı, kiradaki aracı silme, aktif kiralamalı müşteriyi silme ve geçersiz tarih seçimi engellenir.
- **Ekstra Tam Kasko Sigortası & Ciro Takibi:** Kiralama sırasında günlük +250 ₺ Tam Kasko opsiyonu eklenebilir ve Dashboard'da toplam filo cirosu canlı izlenebilir.

---

## 🏛️ Sınıf Diyagramı (UML)

```mermaid
classDiagram
    class IRentable {
        <<interface>>
        +CalculateRentalPrice(int days) decimal
        +MarkAsRented() void
        +MarkAsReturned() void
    }

    class Vehicle {
        <<abstract>>
        -int _id
        -string _plateNumber
        -string _brand
        -string _model
        -int _year
        -decimal _dailyRate
        -VehicleStatus _status
        +VehicleType* string
        +SpecificDetails* string
        +CalculateRentalPrice(int days)* decimal
        +GetPricingPolicyDescription() string
    }

    class Car {
        -string _transmissionType
        +CalculateRentalPrice(int days) decimal
    }

    class Suv {
        -bool _isAllWheelDrive
        +CalculateRentalPrice(int days) decimal
    }

    class ElectricVehicle {
        -int _batteryCapacityKwh
        +CalculateRentalPrice(int days) decimal
    }

    class CommercialVehicle {
        -int _passengerOrLoadCapacity
        +CalculateRentalPrice(int days) decimal
    }

    class Customer {
        -int _id
        -string _firstName
        -string _lastName
        -string _phone
        -string _licenseNumber
        +FullName string
    }

    class Rental {
        -int _id
        -Customer _customer
        -Vehicle _vehicle
        -DateTime _startDate
        -DateTime _endDate
        -int _totalDays
        -decimal _totalPrice
        -bool _hasFullInsurance
        +CompleteReturn() void
    }

    class RentalManager {
        -List~Vehicle~ _vehicles
        -List~Customer~ _customers
        -List~Rental~ _rentals
        +event Action DataChanged
        +AddVehicle(Vehicle)
        +CreateRental(...)
        +ReturnVehicle(int)
    }

    IRentable <|.. Vehicle
    Vehicle <|-- Car
    Vehicle <|-- Suv
    Vehicle <|-- ElectricVehicle
    Vehicle <|-- CommercialVehicle
    RentalManager o-- Vehicle
    RentalManager o-- Customer
    RentalManager o-- Rental
    Rental --> Customer
    Rental --> Vehicle
```

---

## 🧩 OOP İlkelerinin Uygulama Kanıtları

| OOP İlkesi | Projede Uygulanışı |
| :--- | :--- |
| **Kapsülleme (Encapsulation)** | Tüm model sınıflarında (`Vehicle`, `Customer`, `Rental`) alanlar `private` tanımlanmış; `set` bloklarında plaka boşluk, ücret `<= 0`, yıl aralığı ve telefon uzunluğu gibi doğrulamalar yapılmıştır. |
| **Soyutlama (Abstraction)** | `IRentable` arayüzü ve `abstract class Vehicle` üzerinden ortak sözleşme belirlenmiş; `CalculateRentalPrice(int days)` ve `VehicleType` soyut (`abstract`) olarak tanımlanmıştır. |
| **Kalıtım (Inheritance)** | `Car`, `Suv`, `ElectricVehicle` ve `CommercialVehicle` sınıfları `Vehicle` temel sınıfından türetilerek ortak özellikleri miras alır, kendi özel alanlarını ekler. |
| **Çok Biçimlilik (Polymorphism)** | `Car` 7+ günde %10 indirim, `Suv` %15/%8 arazi kaskosu, `ElectricVehicle` %5 çevreci indirim ve `CommercialVehicle` günlük 150 ₺ kurumsal sigorta uygulayarak farklı sonuçlar üretir. |

---

## 📸 Uygulama Ekran Görüntüleri ve Test Senaryoları

### 1. Ana Sayfa (Dashboard)
*Toplam araç, müsait, kirada, müşteri, aktif kiralama ve toplam ciro metrikleri koleksiyonlardan anlık hesaplanır.*
![Dashboard](03_Report/Screenshots/01_dashboard.png)

### 2. Araç Yönetimi
*4 farklı araç sınıfı (`Otomobil`, `SUV`, `Elektrikli`, `Ticari`) ile araç ekleme, arama, durum filtreleme, güncelleme ve silme.*
![Araç Yönetimi](03_Report/Screenshots/02_arac_yonetimi.png)

### 3. Test Senaryosu — Plaka Tekrarı Engeli
*Sistemde kayıtlı bir plaka (`34 SYK 001`) tekrar eklenmeye çalışıldığında `RentalValidationException` yakalanarak uyarı verilir.*
![Plaka Tekrarı Hatası](03_Report/Screenshots/03_plaka_tekrari_hatasi.png)

### 4. Test Senaryosu — Kiradaki Aracı Silme Engeli
*Aktif kirada olan bir araç (`06 ANK 412`) silinmek istendiğinde sistem silme işlemini engeller.*
![Kiradaki Araç Silme Engeli](03_Report/Screenshots/04_kiradaki_arac_silme_engeli.png)

### 5. Müşteri Yönetimi
*Müşteri ekleme, ehliyet numarası tekillik kontrolü, anlık arama ve güncelleme/silme işlemleri.*
![Müşteri Yönetimi](03_Report/Screenshots/05_musteri_yonetimi.png)

### 6. Araç Kiralama & Canlı Polimorfik Fiyat Hesaplama
*Seçilen aracın türüne göre (`Elektrikli Araç: %5 sıfır emisyon indirimi`) sağ panelde ücret ve Tam Kasko tutarı canlı hesaplanır.*
![Araç Kiralama](03_Report/Screenshots/06_arac_kiralama_polimorfizm.png)

### 7. Test Senaryosu — Geçersiz Tarih Doğrulaması
*Bitiş tarihi başlangıç tarihinden önce seçildiğinde kullanıcı anında kırmızı doğrulama mesajı ile uyarılır.*
![Geçersiz Tarih Hatası](03_Report/Screenshots/07_gecersiz_tarih_hatasi.png)

### 8. Test Senaryosu — Kiralama Sözleşmesi Onayı
*Kiralama onaylandığında sözleşme özeti gösterilir ve araç durumu otomatik olarak `Kirada` durumuna geçer.*
![Kiralama Onayı](03_Report/Screenshots/08_kiralama_onayi.png)

### 9. Test Senaryosu — Araç İade İşlemi
*Aktif kiralama seçilip iade alındığında kiralama `Tamamlandı` olur ve araç yeniden `Müsait` statüsüne döner.*
![Araç İade](03_Report/Screenshots/09_arac_iade.png)

---

## 📁 Proje Klasör Yapısı

```text
25905007_SefaYusufKutuk_VehicleRentalSystem/
├── 01_SourceCode/
│   ├── VehicleRentalSystem.sln
│   └── VehicleRentalSystem/
│       ├── Exceptions/
│       │   └── RentalValidationException.cs
│       ├── Models/
│       │   ├── Enums.cs
│       │   ├── IRentable.cs
│       │   ├── Vehicle.cs
│       │   ├── Car.cs
│       │   ├── Suv.cs
│       │   ├── ElectricVehicle.cs
│       │   ├── CommercialVehicle.cs
│       │   ├── Customer.cs
│       │   └── Rental.cs
│       ├── Services/
│       │   └── RentalManager.cs
│       ├── Forms/
│       │   ├── Theme.cs
│       │   ├── MainForm.cs
│       │   ├── DashboardControl.cs
│       │   ├── VehicleManagementControl.cs
│       │   ├── CustomerManagementControl.cs
│       │   ├── RentalControl.cs
│       │   └── ReturnControl.cs
│       └── Program.cs
├── 02_Application/
│   └── Release/
│       └── VehicleRentalSystem.exe
├── 03_Report/
│   ├── Screenshots/
│   └── Proje_Raporu.pdf
├── 04_GitHub/
│   └── README.md
└── README.md
```

---

## 🚀 Kurulum ve Çalıştırma

### Seçenek 1: Doğrudan Çalıştırma (Hazır Release)
`02_Application/Release/VehicleRentalSystem.exe` dosyasına çift tıklayarak uygulamayı doğrudan başlatabilirsiniz. *(Sistemde .NET 10 Windows Desktop Runtime gerektirir.)*

### Seçenek 2: Kaynak Koddan Derleme
```bash
cd 01_SourceCode
dotnet build VehicleRentalSystem.sln -c Release
dotnet run --project VehicleRentalSystem/VehicleRentalSystem.csproj
```
