using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows.TransportationWindows
{
    public partial class TransactionWindow : Window
    {
        public User UserData { get; set; }
        public Actions ActionData { get; set; }
        public MoneyTransaction TransactionData { get; set; }
        public ObservableCollection<MoneyTransaction> TransactionsData { get; set; }


        List<Account> accounts;
        List<JobOrderFinance> jobOrders;
        MoneyTransaction newTransactionData = new MoneyTransaction();
        public TransactionWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Finance].[CompanyAccountsBalancesView] Order By Date Desc";
                accounts = connection.Query<Account>(query).ToList();

                query = "Select * From[Finance].[JobOrdersDetails]";
                jobOrders = connection.Query<JobOrderFinance>(query).ToList();
            }

            newTransactionData.Update(TransactionData);

            AccountsList.ItemsSource = accounts;
            JobOrdersList.ItemsSource = jobOrders;

            if (TransactionData.AccountID == 0)
                if (accounts.Count != 0)
                    newTransactionData.AccountID = accounts[0].ID;


            if (!string.IsNullOrWhiteSpace(newTransactionData.Description))
            {
                string code = newTransactionData.Description.Substring(0, 9);
                newTransactionData.JobOrderID = jobOrders.FirstOrDefault(j => j.Code == code).ID;
                InvoiceNumber.Text = newTransactionData.Description.Substring(32, newTransactionData.Description.Length - 33);
            }

            DataContext = newTransactionData;
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
            bool isReady = false;
            var message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(newTransactionData.Description)) { message += $"\n  Description."; isReady = true; }
            if (newTransactionData.AccountID == 0) { message += $"\n  Account."; isReady = true; }
            if (newTransactionData.Amount == 0) { message += $"\n  Amount."; isReady = true; }

            if (!isReady)
            {
                if (ActionData == Actions.New)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        var query = DatabaseAI.InsertRecord<MoneyTransaction>();
                        newTransactionData.ID = (int)(decimal)connection.ExecuteScalar(query, newTransactionData);
                    }

                    TransactionsData.Insert(0, newTransactionData);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        var query = DatabaseAI.UpdateRecord<MoneyTransaction>();
                        connection.Execute(query, newTransactionData);
                    }

                    TransactionData.Update(newTransactionData);
                }
                this.Close();
            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void AccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountsList.SelectedItem is Account account)
            {
                newTransactionData.AccountName = account.Name;
            }
        }

        private void AccountsList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.ArrowsOnly(e);
        }

        private void JobOrdersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                newTransactionData.JobOrderID = jobOrder.ID;
                newTransactionData.Code = jobOrder.Code;

                string invoiceNumber = InvoiceNumber.Text;
                newTransactionData.Description = $"{jobOrder.Code}-Transportation Invoice {invoiceNumber}";
            }
        }

        private void InvoiceNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                newTransactionData.JobOrderID = jobOrder.ID;
                newTransactionData.Code = jobOrder.Code;

                string invoiceNumber = InvoiceNumber.Text;
                newTransactionData.Description = $"{jobOrder.Code}-Transportation Invoice {invoiceNumber}";
            }
        }
    }
}
