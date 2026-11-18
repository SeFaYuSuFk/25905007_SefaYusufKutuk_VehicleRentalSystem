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
    /// Araç İade modülü: Aktif kiralamaları listeler, seçilen kiralamayı tamamlar ve aracı yeniden müsait yapar.
    /// </summary>
    public class ReturnControl : UserControl
    {
        private readonly RentalManager _manager;
        private readonly DataGridView _dgvRentals = new();
        private readonly CheckBox _chkShowCompleted = new();
        private readonly Label _lblSelectedInfo = new();
        private int? _selectedRentalId = null;

        public ReturnControl(RentalManager manager)
        {
            _manager = manager;
            Dock = DockStyle.Fill;
            BackColor = Theme.ContentBg;
            Padding = new Padding(20);

            InitializeLayout();
            _manager.DataChanged += LoadRentals;
            LoadRentals();
        }

        private void InitializeLayout()
        {
            var lblTitle = new Label
            {
                Text = "Araç İade İşlemleri",
                Font = Theme.TitleFont,
                ForeColor = Theme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 40
            };

            var topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Theme.CardBg,
                Padding = new Padding(16)
            };

            _chkShowCompleted.Text = "Tamamlanmış (İade Edilen) Geçmişi de Göster";
            _chkShowCompleted.Font = Theme.BoldFont;
            _chkShowCompleted.Location = new Point(16, 20);
            _chkShowCompleted.AutoSize = true;
            _chkShowCompleted.CheckedChanged += (s, e) => LoadRentals();

            var btnReturn = new Button
            {
                Text = "Seçili Aracı İade Al",
                Width = 200,
                Height = 38,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(topBar.Width - 216, 13)
            };
            Theme.StyleButton(btnReturn, Theme.PrimaryBlue);
            btnReturn.Click += BtnReturn_Click;

            topBar.Controls.Add(_chkShowCompleted);
            topBar.Controls.Add(btnReturn);

            var gridWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.CardBg,
                Padding = new Padding(12)
            };

            Theme.StyleDataGridView(_dgvRentals);
            _dgvRentals.Dock = DockStyle.Fill;
            _dgvRentals.CellClick += DgvRentals_CellClick;
            gridWrapper.Controls.Add(_dgvRentals);

            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Theme.SidebarBg,
                Padding = new Padding(16, 10, 16, 10)
            };
            _lblSelectedInfo.Text = "İade işlemi için listeden aktif bir kiralama seçin.";
            _lblSelectedInfo.Font = Theme.BoldFont;
            _lblSelectedInfo.ForeColor = Color.White;
            _lblSelectedInfo.Dock = DockStyle.Fill;
            footerPanel.Controls.Add(_lblSelectedInfo);

            Controls.Add(gridWrapper);
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
            Controls.Add(topBar);
            Controls.Add(footerPanel);
            Controls.Add(lblTitle);
        }

        private void LoadRentals()
        {
            _selectedRentalId = null;
            _lblSelectedInfo.Text = "İade işlemi için listeden aktif bir kiralama seçin.";

            var list = (_chkShowCompleted.Checked ? _manager.Rentals : _manager.GetActiveRentals())
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    KiralamaID = r.Id,
                    Müşteri = r.Customer.FullName,
                    Plaka = r.Vehicle.PlateNumber,
                    Araç = $"{r.Vehicle.Brand} {r.Vehicle.Model} ({r.Vehicle.VehicleType})",
                    Başlangıç = r.StartDate.ToString("dd.MM.yyyy"),
                    Bitiş = r.EndDate.ToString("dd.MM.yyyy"),
                    Gün = r.TotalDays,
                    Kasko = r.HasFullInsurance ? "Var" : "Yok",
                    ToplamÜcret = $"{r.TotalPrice:N2} ₺",
                    Durum = r.GetStatusDisplay()
                })
                .ToList();

            _dgvRentals.DataSource = list;
        }

        private void DgvRentals_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(_dgvRentals.Rows[e.RowIndex].Cells["KiralamaID"].Value);
            Rental? rental = _manager.Rentals.FirstOrDefault(r => r.Id == id);
            if (rental == null) return;

            _selectedRentalId = rental.Id;
            _lblSelectedInfo.Text = $"Seçili Kayıt #{rental.Id}: {rental.Vehicle.PlateNumber} - {rental.Customer.FullName} | Tutar: {rental.TotalPrice:N2} ₺ | Durum: {rental.GetStatusDisplay()}";
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedRentalId == null)
                    throw new RentalValidationException("Lütfen tablodan iade alınacak kiralama kaydını seçin.");

                _manager.ReturnVehicle(_selectedRentalId.Value);
                MessageBox.Show("İade işlemi tamamlandı! Kiralama 'Tamamlandı', araç ise yeniden 'Müsait' durumuna getirildi.",
                    "İade Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RentalValidationException ex)
            {
                MessageBox.Show(ex.Message, "İade Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
