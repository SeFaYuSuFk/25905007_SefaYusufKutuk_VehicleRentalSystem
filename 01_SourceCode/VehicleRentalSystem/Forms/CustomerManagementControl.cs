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
    /// Müşteri Yönetimi modülü: Müşteri ekleme, listeleme, arama, güncelleme ve silme.
    /// </summary>
    public class CustomerManagementControl : UserControl
    {
        private readonly RentalManager _manager;
        private int? _selectedCustomerId = null;

        private readonly TextBox _txtFirstName = new();
        private readonly TextBox _txtLastName = new();
        private readonly TextBox _txtPhone = new();
        private readonly TextBox _txtLicense = new();
        private readonly TextBox _txtSearch = new();
        private readonly DataGridView _dgvCustomers = new();

        public CustomerManagementControl(RentalManager manager)
        {
            _manager = manager;
            Dock = DockStyle.Fill;
            BackColor = Theme.ContentBg;
            Padding = new Padding(20);

            InitializeLayout();
            _manager.DataChanged += LoadCustomers;
            LoadCustomers();
        }

        private void InitializeLayout()
        {
            var lblTitle = new Label
            {
                Text = "Müşteri Yönetimi",
                Font = Theme.TitleFont,
                ForeColor = Theme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 40
            };

            var formPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 310,
                BackColor = Theme.CardBg,
                Padding = new Padding(16)
            };

            int y = 16;
            AddField(formPanel, "Ad:", _txtFirstName, ref y);
            AddField(formPanel, "Soyad:", _txtLastName, ref y);
            AddField(formPanel, "Telefon:", _txtPhone, ref y);
            AddField(formPanel, "Ehliyet Numarası:", _txtLicense, ref y);
            y += 10;

            var btnAdd = new Button { Text = "Müşteri Ekle", Location = new Point(16, y), Width = 132 };
            Theme.StyleButton(btnAdd, Theme.SuccessGreen);
            btnAdd.Click += BtnAdd_Click;

            var btnUpdate = new Button { Text = "Güncelle", Location = new Point(160, y), Width = 132 };
            Theme.StyleButton(btnUpdate, Theme.PrimaryBlue);
            btnUpdate.Click += BtnUpdate_Click;
            y += 44;

            var btnDelete = new Button { Text = "Müşteriyi Sil", Location = new Point(16, y), Width = 132 };
            Theme.StyleButton(btnDelete, Theme.DangerRed);
            btnDelete.Click += BtnDelete_Click;

            var btnClear = new Button { Text = "Temizle", Location = new Point(160, y), Width = 132 };
            Theme.StyleButton(btnClear, Theme.TextSecondary);
            btnClear.Click += (s, e) => ClearForm();

            formPanel.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnClear });

            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 0)
            };

            var searchBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Theme.CardBg,
                Padding = new Padding(12, 10, 12, 10)
            };

            var lblSearch = new Label { Text = "Ara (Ad / Soyad / Ehliyet):", Location = new Point(12, 15), AutoSize = true, Font = Theme.BoldFont };
            _txtSearch.Location = new Point(195, 12);
            _txtSearch.Width = 260;
            _txtSearch.Font = Theme.RegularFont;
            _txtSearch.TextChanged += (s, e) => LoadCustomers();
            searchBar.Controls.AddRange(new Control[] { lblSearch, _txtSearch });

            var gridWrapper = new Panel { Dock = DockStyle.Fill, BackColor = Theme.CardBg, Padding = new Padding(12) };
            Theme.StyleDataGridView(_dgvCustomers);
            _dgvCustomers.Dock = DockStyle.Fill;
            _dgvCustomers.CellClick += DgvCustomers_CellClick;
            gridWrapper.Controls.Add(_dgvCustomers);

            listPanel.Controls.Add(gridWrapper);
            listPanel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
            listPanel.Controls.Add(searchBar);

            Controls.Add(listPanel);
            Controls.Add(formPanel);
            Controls.Add(lblTitle);
        }

        private void AddField(Panel panel, string labelText, TextBox tb, ref int y)
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

            tb.Location = new Point(16, y);
            tb.Width = 276;
            tb.Font = Theme.RegularFont;
            y += 36;

            panel.Controls.Add(lbl);
            panel.Controls.Add(tb);
        }

        private void LoadCustomers()
        {
            string q = _txtSearch.Text.Trim().ToLowerInvariant();
            _dgvCustomers.DataSource = _manager.Customers
                .Where(c => string.IsNullOrEmpty(q) ||
                            c.FullName.ToLowerInvariant().Contains(q) ||
                            c.LicenseNumber.ToLowerInvariant().Contains(q) ||
                            c.Phone.Contains(q))
                .Select(c => new
                {
                    ID = c.Id,
                    Ad = c.FirstName,
                    Soyad = c.LastName,
                    Telefon = c.Phone,
                    EhliyetNo = c.LicenseNumber
                })
                .ToList();
        }

        private void DgvCustomers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(_dgvCustomers.Rows[e.RowIndex].Cells["ID"].Value);
            Customer? c = _manager.Customers.FirstOrDefault(x => x.Id == id);
            if (c == null) return;

            _selectedCustomerId = c.Id;
            _txtFirstName.Text = c.FirstName;
            _txtLastName.Text = c.LastName;
            _txtPhone.Text = c.Phone;
            _txtLicense.Text = c.LicenseNumber;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                var customer = new Customer(
                    _manager.GetNextCustomerId(),
                    _txtFirstName.Text,
                    _txtLastName.Text,
                    _txtPhone.Text,
                    _txtLicense.Text);

                _manager.AddCustomer(customer);
                MessageBox.Show($"{customer.FullName} sisteme eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId == null)
                    throw new RentalValidationException("Lütfen tablodan güncellenecek bir müşteri seçin.");

                _manager.UpdateCustomer(
                    _selectedCustomerId.Value,
                    _txtFirstName.Text,
                    _txtLastName.Text,
                    _txtPhone.Text,
                    _txtLicense.Text);

                MessageBox.Show("Müşteri bilgileri güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (_selectedCustomerId == null)
                    throw new RentalValidationException("Lütfen tablodan silinecek bir müşteri seçin.");

                if (MessageBox.Show("Seçili müşteriyi silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _manager.DeleteCustomer(_selectedCustomerId.Value);
                    MessageBox.Show("Müşteri kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            _selectedCustomerId = null;
            _txtFirstName.Clear();
            _txtLastName.Clear();
            _txtPhone.Clear();
            _txtLicense.Clear();
        }
    }
}
