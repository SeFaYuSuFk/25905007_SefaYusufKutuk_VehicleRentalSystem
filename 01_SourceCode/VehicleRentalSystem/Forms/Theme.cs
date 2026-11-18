using System.Drawing;
using System.Windows.Forms;

namespace VehicleRentalSystem.Forms
{
    /// <summary>
    /// Uygulama genelinde tutarlı, modern bir görünüm sağlayan tema ve stil yardımcıları.
    /// </summary>
    public static class Theme
    {
        public static readonly Color SidebarBg = Color.FromArgb(18, 25, 38);       // Koyu Lacivert/Antrasit
        public static readonly Color SidebarActive = Color.FromArgb(37, 99, 235);  // Elektrik Mavi
        public static readonly Color SidebarHover = Color.FromArgb(30, 41, 59);
        public static readonly Color ContentBg = Color.FromArgb(241, 245, 249);    // Açık Slate Gri
        public static readonly Color CardBg = Color.White;
        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        public static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);

        public static readonly Color PrimaryBlue = Color.FromArgb(37, 99, 235);
        public static readonly Color SuccessGreen = Color.FromArgb(22, 163, 74);
        public static readonly Color WarningOrange = Color.FromArgb(234, 88, 12);
        public static readonly Color DangerRed = Color.FromArgb(220, 38, 38);
        public static readonly Color PurpleAccent = Color.FromArgb(124, 58, 237);
        public static readonly Color TealAccent = Color.FromArgb(13, 148, 136);

        public static readonly Font TitleFont = new("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font SubtitleFont = new("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font SectionFont = new("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font RegularFont = new("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font BoldFont = new("Segoe UI", 9.5F, FontStyle.Bold);

        public static void StyleButton(Button btn, Color backColor, Color? foreColor = null)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor ?? Color.White;
            btn.Font = BoldFont;
            btn.Cursor = Cursors.Hand;
            btn.Height = 36;
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 34;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = SidebarBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgv.ColumnHeadersHeight = 38;

            dgv.DefaultCellStyle.Font = RegularFont;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }
    }
}
