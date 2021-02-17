using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows.SuppliersInvoicesWindows
{
    public partial class InvoiceWindow : Window
    {
        public User UserData { get; set; }
        public Database.Suppliers.SupplierInvoice SupplierInvoice { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<MoneyTransaction> transactions;
        public InvoiceWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Finance].[SuppliersInvoicesTransactions] Where SupplierInvoiceID = {SupplierInvoice.ID} And Type ='{MoneyTransactionTypes.SupplierInvoice}'";
                transactions = new ObservableCollection<MoneyTransaction>(connection.Query<MoneyTransaction>(query));
            }
            DataContext = SupplierInvoice;

            viewData = new CollectionViewSource() { Source = transactions };

            TransactionsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (transactions.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = TransactionsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Transactions: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Transaction: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";

            if (transactions != null)
                SupplierInvoice.Paid = transactions.Sum(t => t.Amount);
        }
        private void TransactionsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = TransactionsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Transactions: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Transaction: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var transaction = new MoneyTransaction() { SupplierInvoiceID = SupplierInvoice.ID, SupplierID = SupplierInvoice.SupplierID, JobOrderID = SupplierInvoice.JobOrderID, CustomerID = SupplierInvoice.CustomerID, Date = DateTime.Now, Type = MoneyTransactionTypes.SupplierInvoice.ToString() };
            TransactionWindow jobOrderTransactionWindow = new TransactionWindow()
            {
                UserData = UserData,
                ActionData = Actions.New,
                TransactionData = transaction,
                TransactionsData = transactions,
            };
            jobOrderTransactionWindow.ShowDialog();
        }

        private void Edit_ClicK(object sender, RoutedEventArgs e)
        {
            if (TransactionsList.SelectedItem is MoneyTransaction transaction)
            {
                TransactionWindow jobOrderTransactionWindow = new TransactionWindow()
                {
                    UserData = UserData,
                    ActionData = Actions.New,
                    TransactionData = transaction,
                    TransactionsData = transactions,
                };
                jobOrderTransactionWindow.ShowDialog();

                if (transactions != null)
                    SupplierInvoice.Paid = transactions.Sum(t => t.Amount);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsList.SelectedItem is MoneyTransaction transaction)
            {
                MessageBoxResult result = CMessageBox.Show("Deleting", $"Do you want to delete: \n{transaction.Description}?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute($"Delete From [Finance].[MoneyTransactions] Where ID = {transaction.ID}");
                        transactions.Remove(transaction);
                    }
                }
            }
        }
    }
}
