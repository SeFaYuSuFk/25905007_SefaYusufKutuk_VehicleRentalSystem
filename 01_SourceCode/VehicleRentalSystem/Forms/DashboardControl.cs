using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Ana Sayfa (Dashboard): Koleksiyonlardan dinamik hesaplanan özet istatistik kartları ve aktif kiralamalar.
    /// </summary>
    public class DashboardControl : UserControl
    {
        private readonly RentalManager _manager;
        private readonly Label _lblTotalVehiclesVal = new();
        private readonly Label _lblAvailableVehiclesVal = new();
        private readonly Label _lblRentedVehiclesVal = new();
        private readonly Label _lblTotalCustomersVal = new();
        private readonly Label _lblActiveRentalsVal = new();
        private readonly Label _lblTotalRevenueVal = new();
        private readonly DataGridView _dgvRecentRentals = new();

        public DashboardControl(RentalManager manager)
        {
            _manager = manager;
            Dock = DockStyle.Fill;
            BackColor = Theme.ContentBg;
            Padding = new Padding(20);

            InitializeLayout();
            _manager.DataChanged += RefreshDashboard;
            RefreshDashboard();
        }

        private void InitializeLayout()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            var lblTitle = new Label
            {
                Text = "Genel Bakış (Dashboard)",
                Font = Theme.TitleFont,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            var lblSubtitle = new Label
            {
                Text = "Filo durumu, müşteri sayısı ve aktif kiralama metrikleri anlık olarak hesaplanır.",
                Font = Theme.SubtitleFont,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(0, 32)
            };

            var btnResetSample = new Button
            {
                Text = "Örnek Verileri Sıfırla",
                Width = 170,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(headerPanel.Width - 175, 8)
            };
            Theme.StyleButton(btnResetSample, Theme.SidebarHover);
            btnResetSample.Click += (s, e) =>
            {
                if (MessageBox.Show("Tüm veriler başlangıç örnek verilerine sıfırlansın mı?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _manager.SeedSampleData();
                }
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(btnResetSample);

            // 6 İstatistik Kartı Grid'i (2 satır x 3 sütun)
            var cardsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 230,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(0, 10, 0, 10)
            };
            for (int c = 0; c < 3; c++) cardsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            for (int r = 0; r < 2; r++) cardsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            cardsTable.Controls.Add(CreateStatCard("Toplam Araç", _lblTotalVehiclesVal, Theme.PrimaryBlue, "Filodaki tüm kayıtlı araçlar"), 0, 0);
            cardsTable.Controls.Add(CreateStatCard("Müsait Araç", _lblAvailableVehiclesVal, Theme.SuccessGreen, "Kiralamaya hazır araçlar"), 1, 0);
            cardsTable.Controls.Add(CreateStatCard("Kiradaki Araç", _lblRentedVehiclesVal, Theme.WarningOrange, "Şu anda müşteride olanlar"), 2, 0);
            cardsTable.Controls.Add(CreateStatCard("Toplam Müşteri", _lblTotalCustomersVal, Theme.PurpleAccent, "Sistemdeki kayıtlı müşteriler"), 0, 1);
            cardsTable.Controls.Add(CreateStatCard("Aktif Kiralama", _lblActiveRentalsVal, Theme.DangerRed, "İadesi beklenen sözleşmeler"), 1, 1);
            cardsTable.Controls.Add(CreateStatCard("Toplam Ciro", _lblTotalRevenueVal, Theme.TealAccent, "Tüm kiralamaların toplam tutarı"), 2, 1);

            // Alt Kısım: Aktif Kiralamalar Özeti
            var tableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.CardBg,
                Padding = new Padding(15)
            };
            var lblGridTitle = new Label
            {
                Text = "Son Kiralama Kayıtları",
                Font = Theme.SectionFont,
                ForeColor = Theme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 32
            };

            Theme.StyleDataGridView(_dgvRecentRentals);
            _dgvRecentRentals.Dock = DockStyle.Fill;

            tableContainer.Controls.Add(_dgvRecentRentals);
            tableContainer.Controls.Add(lblGridTitle);

            Controls.Add(tableContainer);
            Controls.Add(cardsTable);
            Controls.Add(headerPanel);
        }

        private Panel CreateStatCard(string title, Label valueLabel, Color accentColor, string hint)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.CardBg,
                Margin = new Padding(6),
                Padding = new Padding(14)
            };

            var accentBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = accentColor
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = Theme.BoldFont,
                ForeColor = Theme.TextSecondary,
                Location = new Point(18, 10),
                AutoSize = true
            };

            valueLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            valueLabel.ForeColor = Theme.TextPrimary;
            valueLabel.Location = new Point(16, 34);
            valueLabel.AutoSize = true;

            var lblHint = new Label
            {
                Text = hint,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Theme.TextSecondary,
                Location = new Point(18, 76),
                AutoSize = true
            };

            card.Controls.Add(lblHint);
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);
            card.Controls.Add(accentBar);
            return card;
        }

        public void RefreshDashboard()
        {
            _lblTotalVehiclesVal.Text = _manager.TotalVehicleCount.ToString();
            _lblAvailableVehiclesVal.Text = _manager.AvailableVehicleCount.ToString();
            _lblRentedVehiclesVal.Text = _manager.RentedVehicleCount.ToString();
            _lblTotalCustomersVal.Text = _manager.TotalCustomerCount.ToString();
            _lblActiveRentalsVal.Text = _manager.ActiveRentalCount.ToString();
            _lblTotalRevenueVal.Text = $"{_manager.TotalRevenue:N0} ₺";

            _dgvRecentRentals.DataSource = _manager.Rentals
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    KiralamaNo = r.Id,
                    Müşteri = r.Customer.FullName,
                    Araç = $"{r.Vehicle.PlateNumber} - {r.Vehicle.Brand} {r.Vehicle.Model}",
                    Tür = r.Vehicle.VehicleType,
                    Başlangıç = r.StartDate.ToString("dd.MM.yyyy"),
                    Bitiş = r.EndDate.ToString("dd.MM.yyyy"),
                    Gün = r.TotalDays,
                    ToplamTutar = $"{r.TotalPrice:N2} ₺",
                    Durum = r.GetStatusDisplay()
                })
                .ToList();
        }
    }
}
