using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VehicleRentalSystem.Exceptions;
using VehicleRentalSystem.Models;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Araç Yönetimi modülü: Araç ekleme, listeleme, filtreleme, güncelleme ve silme işlemleri.
    /// </summary>
    public class VehicleManagementControl : UserControl
    {
        private readonly RentalManager _manager;
        private int? _selectedVehicleId = null;

        private readonly ComboBox _cmbType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox _txtPlate = new();
        private readonly TextBox _txtBrand = new();
        private readonly TextBox _txtModel = new();
        private readonly NumericUpDown _numYear = new() { Minimum = 1950, Maximum = 2030, Value = 2024 };
        private readonly NumericUpDown _numDailyRate = new() { Minimum = 1, Maximum = 100000, DecimalPlaces = 2, Value = 1500 };
        private readonly Label _lblExtra = new();
        private readonly TextBox _txtExtra = new();

        private readonly TextBox _txtSearch = new();
        private readonly ComboBox _cmbFilterStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DataGridView _dgvVehicles = new();

        public VehicleManagementControl(RentalManager manager)
        {
            _manager = manager;
            Dock = DockStyle.Fill;
            BackColor = Theme.ContentBg;
            Padding = new Padding(20);

            InitializeLayout();
            _manager.DataChanged += LoadVehicles;
            LoadVehicles();
        }

        private void InitializeLayout()
        {
            var lblTitle = new Label
            {
                Text = "Araç Yönetimi",
                Font = Theme.TitleFont,
                ForeColor = Theme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 40
            };

            // Sol Form Paneli
            var formPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 310,
                BackColor = Theme.CardBg,
                Padding = new Padding(16)
            };

            int y = 14;
            AddField(formPanel, "Araç Türü:", _cmbType, ref y);
            _cmbType.Items.AddRange(new object[] { "Otomobil", "SUV / 4x4", "Elektrikli", "Ticari / VIP" });
            _cmbType.SelectedIndex = 0;
            _cmbType.SelectedIndexChanged += (s, e) => UpdateExtraLabel();

            AddField(formPanel, "Plaka (Örn: 34 ABC 123):", _txtPlate, ref y);
            AddField(formPanel, "Marka:", _txtBrand, ref y);
            AddField(formPanel, "Model:", _txtModel, ref y);
            AddField(formPanel, "Üretim Yılı:", _numYear, ref y);
            AddField(formPanel, "Günlük Kira Ücreti (₺):", _numYear == null ? _numDailyRate : _numDailyRate, ref y);

            _lblExtra.Font = Theme.BoldFont;
            _lblExtra.ForeColor = Theme.TextPrimary;
            _lblExtra.Location = new Point(16, y);
            _lblExtra.AutoSize = true;
            y += 22;

            _txtExtra.Location = new Point(16, y);
            _txtExtra.Width = 276;
            _txtExtra.Font = Theme.RegularFont;
            y += 38;

            formPanel.Controls.Add(_lblExtra);
            formPanel.Controls.Add(_txtExtra);
            UpdateExtraLabel();

            var btnAdd = new Button { Text = "Araç Ekle", Location = new Point(16, y), Width = 132 };
            Theme.StyleButton(btnAdd, Theme.SuccessGreen);
            btnAdd.Click += BtnAdd_Click;

            var btnUpdate = new Button { Text = "Güncelle", Location = new Point(160, y), Width = 132 };
            Theme.StyleButton(btnUpdate, Theme.PrimaryBlue);
            btnUpdate.Click += BtnUpdate_Click;
            y += 44;

            var btnDelete = new Button { Text = "Seçili Aracı Sil", Location = new Point(16, y), Width = 132 };
            Theme.StyleButton(btnDelete, Theme.DangerRed);
            btnDelete.Click += BtnDelete_Click;

            var btnClear = new Button { Text = "Temizle", Location = new Point(160, y), Width = 132 };
            Theme.StyleButton(btnClear, Theme.TextSecondary);
            btnClear.Click += (s, e) => ClearForm();

            formPanel.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnClear });

            // Sağ Liste Paneli
            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 0)
            };

            var filterBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Theme.CardBg,
                Padding = new Padding(12, 10, 12, 10)
            };

            var lblSearch = new Label { Text = "Ara (Plaka/Marka):", Location = new Point(12, 15), AutoSize = true, Font = Theme.BoldFont };
            _txtSearch.Location = new Point(140, 12);
            _txtSearch.Width = 200;
            _txtSearch.Font = Theme.RegularFont;
            _txtSearch.TextChanged += (s, e) => LoadVehicles();

            var lblStatus = new Label { Text = "Durum:", Location = new Point(360, 15), AutoSize = true, Font = Theme.BoldFont };
            _cmbFilterStatus.Location = new Point(415, 12);
            _cmbFilterStatus.Width = 140;
            _cmbFilterStatus.Font = Theme.RegularFont;
            _cmbFilterStatus.Items.AddRange(new object[] { "Tümü", "Müsait", "Kirada" });
            _cmbFilterStatus.SelectedIndex = 0;
            _cmbFilterStatus.SelectedIndexChanged += (s, e) => LoadVehicles();

            filterBar.Controls.AddRange(new Control[] { lblSearch, _txtSearch, lblStatus, _cmbFilterStatus });

            var gridWrapper = new Panel { Dock = DockStyle.Fill, BackColor = Theme.CardBg, Padding = new Padding(12) };
            Theme.StyleDataGridView(_dgvVehicles);
            _dgvVehicles.Dock = DockStyle.Fill;
            _dgvVehicles.CellClick += DgvVehicles_CellClick;
            gridWrapper.Controls.Add(_dgvVehicles);

            listPanel.Controls.Add(gridWrapper);
            listPanel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
            listPanel.Controls.Add(filterBar);

            Controls.Add(listPanel);
            Controls.Add(lblTitle);
            formPanel.BringToFront();
            listPanel.BringToFront();
            Controls.Add(formPanel);
            Controls.SetChildIndex(lblTitle, Controls.Count - 1);
        }

        private void AddField(Panel panel, string labelText, Control input, ref int y)
        {
            var lbl = new Label
            {
                Text = labelText,
                Font = Theme.BoldFont,
                ForeColor = Theme.TextPrimary,
                Location = new Point(16, y),
                AutoSize = true
            };
            y += 22;

            input.Location = new Point(16, y);
            input.Width = 276;
            input.Font = Theme.RegularFont;
            y += 34;

            panel.Controls.Add(lbl);
            panel.Controls.Add(input);
        }

        private void UpdateExtraLabel()
        {
            switch (_cmbType.SelectedItem?.ToString())
            {
                case "Otomobil":
                    _lblExtra.Text = "Vites Tipi (Otomatik/Manuel):";
                    if (string.IsNullOrWhiteSpace(_txtExtra.Text)) _txtExtra.Text = "Otomatik";
                    break;
                case "SUV / 4x4":
                    _lblExtra.Text = "Çekiş Sistemi (4x4 / 4x2):";
                    _txtExtra.Text = "4x4";
                    break;
                case "Elektrikli":
                    _lblExtra.Text = "Batarya Kapasitesi (kWh):";
                    _txtExtra.Text = "80";
                    break;
                case "Ticari / VIP":
                    _lblExtra.Text = "Yolcu / Yük Kapasitesi:";
                    _txtExtra.Text = "9";
                    break;
            }
        }

        private void LoadVehicles()
        {
            string query = _txtSearch.Text.Trim().ToLowerInvariant();
            string statusFilter = _cmbFilterStatus.SelectedItem?.ToString() ?? "Tümü";

            var filtered = _manager.Vehicles.Where(v =>
            {
                bool matchesQuery = string.IsNullOrEmpty(query) ||
                                    v.PlateNumber.ToLowerInvariant().Contains(query) ||
                                    v.Brand.ToLowerInvariant().Contains(query) ||
                                    v.Model.ToLowerInvariant().Contains(query);

                bool matchesStatus = statusFilter == "Tümü" || v.GetStatusDisplay() == statusFilter;
                return matchesQuery && matchesStatus;
            })
            .Select(v => new
            {
                ID = v.Id,
                Plaka = v.PlateNumber,
                Tür = v.VehicleType,
                Marka = v.Brand,
                Model = v.Model,
                Yıl = v.Year,
                GünlükÜcret = $"{v.DailyRate:N2} ₺",
                Özellik = v.SpecificDetails,
                Durum = v.GetStatusDisplay()
            })
            .ToList();

            _dgvVehicles.DataSource = filtered;
        }

        private void DgvVehicles_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(_dgvVehicles.Rows[e.RowIndex].Cells["ID"].Value);
            Vehicle? v = _manager.Vehicles.FirstOrDefault(x => x.Id == id);
            if (v == null) return;

            _selectedVehicleId = v.Id;
            _cmbType.SelectedItem = v.VehicleType;
            _cmbType.Enabled = false; // Mevcut aracın sınıf tipi değiştirilmez
            _txtPlate.Text = v.PlateNumber;
            _txtBrand.Text = v.Brand;
            _txtBrand.Focus();
            _txtModel.Text = v.Model;
            _numYear.Value = v.Year;
            _numDailyRate.Value = v.DailyRate;

            _txtExtra.Text = v switch
            {
                Car c => c.TransmissionType,
                Suv s => s.IsAllWheelDrive ? "4x4" : "4x2",
                ElectricVehicle ev => ev.BatteryCapacityKwh.ToString(),
                CommercialVehicle cv => cv.PassengerOrLoadCapacity.ToString(),
                _ => ""
            };
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                int id = _manager.GetNextVehicleId();
                string plate = _txtPlate.Text;
                string brand = _txtBrand.Text;
                string model = _txtModel.Text;
                int year = (int)_numYear.Value;
                decimal rate = _numDailyRate.Value;
                string extra = _txtExtra.Text.Trim();

                Vehicle newVehicle = _cmbType.SelectedItem?.ToString() switch
                {
                    "SUV / 4x4" => new Suv(id, plate, brand, model, year, rate, extra.Contains("4x4", StringComparison.OrdinalIgnoreCase)),
                    "Elektrikli" => new ElectricVehicle(id, plate, brand, model, year, rate, int.TryParse(extra, out int kwh) ? kwh : 75),
                    "Ticari / VIP" => new CommercialVehicle(id, plate, brand, model, year, rate, int.TryParse(extra, out int cap) ? cap : 8),
                    _ => new Car(id, plate, brand, model, year, rate, string.IsNullOrWhiteSpace(extra) ? "Otomatik" : extra)
                };

                _manager.AddVehicle(newVehicle);
                MessageBox.Show($"{newVehicle.PlateNumber} plakalı {newVehicle.VehicleType} başarıyla eklendi.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (RentalValidationException ex)
            {
                MessageBox.Show(ex.Message, "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedVehicleId == null)
                    throw new RentalValidationException("Lütfen tablodan güncellenecek bir araç seçin.");

                _manager.UpdateVehicle(
                    _selectedVehicleId.Value,
                    _txtPlate.Text,
                    _txtBrand.Text,
                    _txtModel.Text,
                    (int)_numYear.Value,
                    _numDailyRate.Value,
                    _txtExtra.Text);

                MessageBox.Show("Araç bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (RentalValidationException ex)
            {
                MessageBox.Show(ex.Message, "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedVehicleId == null)
                    throw new RentalValidationException("Lütfen tablodan silinecek bir araç seçin.");

                if (MessageBox.Show("Seçili aracı silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _manager.DeleteVehicle(_selectedVehicleId.Value);
                    MessageBox.Show("Araç kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (RentalValidationException ex)
            {
                MessageBox.Show(ex.Message, "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            _selectedVehicleId = null;
            _cmbType.Enabled = true;
            _cmbType.SelectedIndex = 0;
            _txtPlate.Clear();
            _txtBrand.Clear();
            _txtModel.Clear();
            _numYear.Value = 2024;
            _numDailyRate.Value = 1500;
            UpdateExtraLabel();
        }
    }
}
