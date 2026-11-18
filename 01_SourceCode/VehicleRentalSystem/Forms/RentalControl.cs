using System;
using System.Drawing;
using System.Windows.Forms;
using VehicleRentalSystem.Exceptions;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Araç Kiralama modülü: Müşteri ve müsait araç seçimi, tarih doğrulama ve polimorfik fiyat hesaplama.
    /// </summary>
    public class RentalControl : UserControl
    {
        private readonly RentalManager _manager;

        private readonly ComboBox _cmbCustomers = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox _cmbVehicles = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DateTimePicker _dtpStart = new() { Format = DateTimePickerFormat.Short };
        private readonly DateTimePicker _dtpEnd = new() { Format = DateTimePickerFormat.Short };
        private readonly CheckBox _chkFullInsurance = new();

        private readonly Label _lblPolicy = new();
        private readonly Label _lblDaysValue = new();
        private readonly Label _lblVehiclePriceValue = new();
        private readonly Label _lblInsurancePriceValue = new();
        private readonly Label _lblTotalPriceValue = new();
        private readonly Label _lblDateWarning = new();

        public RentalControl(RentalManager manager)
        {
            _manager = manager;
            Dock = DockStyle.Fill;
            BackColor = Theme.ContentBg;
            Padding = new Padding(20);

            InitializeLayout();
            _manager.DataChanged += RefreshSelections;
            RefreshSelections();
        }

        private void InitializeLayout()
        {
            var lblTitle = new Label
            {
                Text = "Araç Kiralama İşlemi",
                Font = Theme.TitleFont,
                ForeColor = Theme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 40
            };

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 10, 0, 0)
            };
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

            // Sol Panel: Kiralama Formu
            var formCard = new Panel { Dock = DockStyle.Fill, BackColor = Theme.CardBg, Padding = new Padding(24), Margin = new Padding(0, 0, 10, 0) };
            int y = 20;

            AddLabel(formCard, "1. Müşteri Seçimi:", ref y);
            _cmbCustomers.Location = new Point(24, y);
            _cmbCustomers.Width = 380;
            _cmbCustomers.Font = Theme.RegularFont;
            formCard.Controls.Add(_cmbCustomers);
            y += 45;

            AddLabel(formCard, "2. Müsait Araç Seçimi:", ref y);
            _cmbVehicles.Location = new Point(24, y);
            _cmbVehicles.Width = 380;
            _cmbVehicles.Font = Theme.RegularFont;
            _cmbVehicles.SelectedIndexChanged += (s, e) => RecalculateSummary();
            formCard.Controls.Add(_cmbVehicles);
            y += 45;

            AddLabel(formCard, "3. Kiralama Başlangıç Tarihi:", ref y);
            _dtpStart.Location = new Point(24, y);
            _dtpStart.Width = 220;
            _dtpStart.Font = Theme.RegularFont;
            _dtpStart.Value = DateTime.Today;
            _dtpStart.ValueChanged += (s, e) => RecalculateSummary();
            formCard.Controls.Add(_dtpStart);
            y += 45;

            AddLabel(formCard, "4. Kiralama Bitiş Tarihi:", ref y);
            _dtpEnd.Location = new Point(24, y);
            _dtpEnd.Width = 220;
            _dtpEnd.Font = Theme.RegularFont;
            _dtpEnd.Value = DateTime.Today.AddDays(3);
            _dtpEnd.ValueChanged += (s, e) => RecalculateSummary();
            formCard.Controls.Add(_dtpEnd);
            y += 40;

            _chkFullInsurance.Text = $"Ekstra Tam Kasko Sigortası (+{Rental.DailyInsuranceFee:N0} ₺ / gün)";
            _chkFullInsurance.Font = Theme.BoldFont;
            _chkFullInsurance.ForeColor = Theme.TextPrimary;
            _chkFullInsurance.Location = new Point(24, y);
            _chkFullInsurance.AutoSize = true;
            _chkFullInsurance.CheckedChanged += (s, e) => RecalculateSummary();
            formCard.Controls.Add(_chkFullInsurance);
            y += 40;

            _lblDateWarning.ForeColor = Theme.DangerRed;
            _lblDateWarning.Font = Theme.BoldFont;
            _lblDateWarning.Location = new Point(24, y);
            _lblDateWarning.AutoSize = true;
            formCard.Controls.Add(_lblDateWarning);
            y += 32;

            var btnConfirm = new Button
            {
                Text = "Kiralamayı Onayla ve Sözleşme Oluştur",
                Location = new Point(24, y),
                Width = 380,
                Height = 44
            };
            Theme.StyleButton(btnConfirm, Theme.SuccessGreen);
            btnConfirm.Click += BtnConfirm_Click;
            formCard.Controls.Add(btnConfirm);

            // Sağ Panel: Canlı Polimorfik Fiyat Özeti
            var summaryCard = new Panel { Dock = DockStyle.Fill, BackColor = Theme.SidebarBg, Padding = new Padding(24), Margin = new Padding(10, 0, 0, 0) };
            var lblSummaryTitle = new Label
            {
                Text = "Kiralama Ücret Özeti",
                Font = Theme.TitleFont,
                ForeColor = Color.White,
                Location = new Point(24, 24),
                AutoSize = true
            };

            _lblPolicy.Font = Theme.RegularFont;
            _lblPolicy.ForeColor = Color.FromArgb(147, 197, 253);
            _lblPolicy.Location = new Point(24, 68);
            _lblPolicy.MaximumSize = new Size(340, 0);
            _lblPolicy.AutoSize = true;

            int sy = 135;
            AddSummaryRow(summaryCard, "Kiralama Süresi:", _lblDaysValue, ref sy);
            AddSummaryRow(summaryCard, "Araç Kira Bedeli (Polimorfik):", _lblVehiclePriceValue, ref sy);
            AddSummaryRow(summaryCard, "Tam Kasko Sigortası:", _lblInsurancePriceValue, ref sy);

            var divider = new Panel { BackColor = Color.FromArgb(51, 65, 85), Location = new Point(24, sy), Width = 340, Height = 2 };
            summaryCard.Controls.Add(divider);
            sy += 18;

            var lblTotalText = new Label { Text = "TOPLAM TUTAR", Font = Theme.BoldFont, ForeColor = Color.FromArgb(148, 163, 184), Location = new Point(24, sy), AutoSize = true };
            _lblTotalPriceValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            _lblTotalPriceValue.ForeColor = Color.FromArgb(74, 222, 128);
            _lblTotalPriceValue.Location = new Point(24, sy + 26);
            _lblTotalPriceValue.AutoSize = true;

            summaryCard.Controls.AddRange(new Control[] { lblSummaryTitle, _lblPolicy, lblTotalText, _lblTotalPriceValue });

            container.Controls.Add(formCard, 0, 0);
            container.Controls.Add(summaryCard, 1, 0);

            Controls.Add(container);
            Controls.Add(lblTitle);
        }

        private void AddLabel(Panel p, string text, ref int y)
        {
            var lbl = new Label { Text = text, Font = Theme.BoldFont, ForeColor = Theme.TextPrimary, Location = new Point(24, y), AutoSize = true };
            p.Controls.Add(lbl);
            y += 24;
        }

        private void AddSummaryRow(Panel p, string title, Label valueLabel, ref int y)
        {
            var lbl = new Label { Text = title, Font = Theme.RegularFont, ForeColor = Color.FromArgb(203, 213, 225), Location = new Point(24, y), AutoSize = true };
            valueLabel.Font = Theme.BoldFont;
            valueLabel.ForeColor = Color.White;
            valueLabel.Location = new Point(24, y + 20);
            valueLabel.AutoSize = true;
            p.Controls.Add(lbl);
            p.Controls.Add(valueLabel);
            y += 54;
        }

        public void RefreshSelections()
        {
            _cmbCustomers.DataSource = null;
            _cmbCustomers.DataSource = _manager.Customers;
            _cmbCustomers.DisplayMember = "FullName";

            var available = _manager.GetAvailableVehicles();
            _cmbVehicles.DataSource = null;
            _cmbVehicles.DataSource = available;

            RecalculateSummary();
        }

        private void RecalculateSummary()
        {
            _lblDateWarning.Text = string.Empty;

            if (_dtpEnd.Value.Date < _dtpStart.Value.Date)
            {
                _lblDateWarning.Text = "Uyarı: Bitiş tarihi başlangıç tarihinden önce olamaz!";
                _lblDaysValue.Text = "-";
                _lblVehiclePriceValue.Text = "-";
                _lblInsurancePriceValue.Text = "-";
                _lblTotalPriceValue.Text = "0,00 ₺";
                return;
            }

            int diff = (_dtpEnd.Value.Date - _dtpStart.Value.Date).Days;
            int days = diff == 0 ? 1 : diff;
            _lblDaysValue.Text = $"{days} Gün";

            if (_cmbVehicles.SelectedItem is not Vehicle vehicle)
            {
                _lblPolicy.Text = "Müsait araç bulunamadı.";
                _lblVehiclePriceValue.Text = "0,00 ₺";
                _lblInsurancePriceValue.Text = "0,00 ₺";
                _lblTotalPriceValue.Text = "0,00 ₺";
                return;
            }

            _lblPolicy.Text = vehicle.GetPricingPolicyDescription();
            decimal vehiclePrice = vehicle.CalculateRentalPrice(days);
            decimal insurancePrice = _chkFullInsurance.Checked ? days * Rental.DailyInsuranceFee : 0m;
            decimal total = vehiclePrice + insurancePrice;

            _lblVehiclePriceValue.Text = $"{vehiclePrice:N2} ₺";
            _lblInsurancePriceValue.Text = $"{insurancePrice:N2} ₺";
            _lblTotalPriceValue.Text = $"{total:N2} ₺";
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            try
            {
                var customer = _cmbCustomers.SelectedItem as Customer;
                var vehicle = _cmbVehicles.SelectedItem as Vehicle;

                Rental rental = _manager.CreateRental(
                    customer!,
                    vehicle!,
                    _dtpStart.Value,
                    _dtpEnd.Value,
                    _chkFullInsurance.Checked);

                string summary =
                    $"Kiralama Sözleşmesi #{rental.Id} Oluşturuldu!\n\n" +
                    $"Müşteri: {rental.Customer.FullName}\n" +
                    $"Araç: {rental.Vehicle.PlateNumber} - {rental.Vehicle.Brand} {rental.Vehicle.Model}\n" +
                    $"Tarih: {rental.StartDate:dd.MM.yyyy} - {rental.EndDate:dd.MM.yyyy} ({rental.TotalDays} Gün)\n" +
                    $"Tam Kasko: {(rental.HasFullInsurance ? "Evet" : "Hayır")}\n" +
                    $"Toplam Tutar: {rental.TotalPrice:N2} ₺\n\n" +
                    $"Araç durumu 'Kirada' olarak güncellendi.";

                MessageBox.Show(summary, "Kiralama Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RentalValidationException ex)
            {
                MessageBox.Show(ex.Message, "Kiralama Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
