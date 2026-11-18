using System;
using System.Drawing;
using System.Windows.Forms;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Uygulamanın ana çerçeve formu. Sol yan menü üzerinden tüm modüllere geçiş sağlar.
    /// </summary>
    public class MainForm : Form
    {
        private readonly RentalManager _manager;
        private readonly Panel _contentPanel = new() { Dock = DockStyle.Fill };

        private readonly DashboardControl _dashboardControl;
        private readonly VehicleManagementControl _vehicleControl;
        private readonly CustomerManagementControl _customerControl;
        private readonly RentalControl _rentalControl;
        private readonly ReturnControl _returnControl;

        private Button? _activeButton;

        public MainForm()
        {
            _manager = new RentalManager(loadSampleData: true);

            Text = "SYK Fleet | Araç Kiralama Yönetim Sistemi";
            Size = new Size(1220, 750);
            MinimumSize = new Size(1024, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.ContentBg;

            _dashboardControl = new DashboardControl(_manager);
            _vehicleControl = new VehicleManagementControl(_manager);
            _customerControl = new CustomerManagementControl(_manager);
            _rentalControl = new RentalControl(_manager);
            _returnControl = new ReturnControl(_manager);

            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 235,
                BackColor = Theme.SidebarBg
            };

            var brandPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(18, 20, 18, 10)
            };

            var lblBrand = new Label
            {
                Text = "SYK FLEET",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(18, 18),
                AutoSize = true
            };

            var lblAppSub = new Label
            {
                Text = "Araç Kiralama Sistemi",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 48),
                AutoSize = true
            };
            brandPanel.Controls.AddRange(new Control[] { lblBrand, lblAppSub });

            var navPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };

            var btnReturn = CreateNavButton("  Araç İade", () => LoadControl(_returnControl));
            var btnRental = CreateNavButton("  Araç Kiralama", () => LoadControl(_rentalControl));
            var btnCustomer = CreateNavButton("  Müşteri Yönetimi", () => LoadControl(_customerControl));
            var btnVehicle = CreateNavButton("  Araç Yönetimi", () => LoadControl(_vehicleControl));
            var btnDashboard = CreateNavButton("  Ana Sayfa (Dashboard)", () => LoadControl(_dashboardControl));

            // Dock = Top ekleme sırası alttan üste doğru dizilir
            navPanel.Controls.Add(btnReturn);
            navPanel.Controls.Add(btnRental);
            navPanel.Controls.Add(btnCustomer);
            navPanel.Controls.Add(btnVehicle);
            navPanel.Controls.Add(btnDashboard);

            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                Padding = new Padding(16, 10, 16, 10),
                BackColor = Color.FromArgb(15, 20, 31)
            };
            var lblStudent = new Label
            {
                Text = "Sefa Yusuf Kütük\nNo: 25905007 • BGY203",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill
            };
            footerPanel.Controls.Add(lblStudent);

            sidebar.Controls.Add(navPanel);
            sidebar.Controls.Add(brandPanel);
            sidebar.Controls.Add(footerPanel);

            Controls.Add(_contentPanel);
            Controls.Add(sidebar);

            // Başlangıç ekranı olarak Dashboard açılır
            SetActiveButton(btnDashboard);
            LoadControl(_dashboardControl);
        }

        private Button CreateNavButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = Theme.BoldFont,
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Theme.SidebarBg,
                Cursor = Cursors.Hand,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(0, 4, 0, 4)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;

            btn.Click += (s, e) =>
            {
                SetActiveButton(btn);
                onClick();
            };
            return btn;
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = Theme.SidebarBg;
                _activeButton.ForeColor = Color.FromArgb(203, 213, 225);
            }

            _activeButton = btn;
            _activeButton.BackColor = Theme.SidebarActive;
            _activeButton.ForeColor = Color.White;
        }

        private void LoadControl(UserControl control)
        {
            _contentPanel.Controls.Clear();
            _contentPanel.Controls.Add(control);
            control.Focus();
        }
    }
}
