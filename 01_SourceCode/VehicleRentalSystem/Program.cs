using System;
using System.Windows.Forms;
using VehicleRentalSystem.Forms;

namespace VehicleRentalSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
