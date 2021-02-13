using Dapper;
using System.Windows;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows
{
    public partial class BankInformationWindow : Window
    {
        public BankAccount BankData { get; set; }

        BankAccount bankData = new BankAccount();
        public BankInformationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (BankData != null)
                bankData.Update(BankData);

            DataContext = bankData;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isReady = true;
            var message = "Please Enter:";

            if (string.IsNullOrWhiteSpace(bankData.Name)) { message += $"\n  Name."; isReady = false; }
            if (string.IsNullOrWhiteSpace(bankData.Number)) { message += $"\n  Number."; isReady = false; }
            if (string.IsNullOrWhiteSpace(bankData.IBAN)) { message += $"\n  IBAN."; isReady = false; }

            if (!isReady)
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
                return;
            }

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"{DatabaseAI.UpdateRecord<BankAccount>()}";
                connection.Execute(query, bankData);
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
