using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace ProjectsNow
{
    public partial class App : Application
    {
        public static double VAT { get; set; }
        public static readonly double cm = 37.7952755905512;
        public static readonly double A4Width = 29.7 * cm;
        public static readonly double A4Height = 21 * cm;

        public static string computerName = Environment.MachineName.ToString();
        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Process myProcess = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(pcProcess =>
                pcProcess.ProcessName == myProcess.ProcessName).Count();

            if (count > 1)
            {
                MessageBox.Show("Application is running...");
                App.Current.Shutdown();
            }
        }
    }
}
