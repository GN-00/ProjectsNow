using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows.TransportationWindows
{
    public partial class TransportationWindow : Window
    {
        public User UserData { get; set; }
        public JobOrderFinance JobOrderData { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<MoneyTransaction> transactions;
        public TransportationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (JobOrderData != null)
                    query = $"Select * From [Finance].[TransportationView] Where JobOrderID = {JobOrderData.ID} And Type ='{MoneyTransactionTypes.Transportation}'";
                else
                    query = $"Select * From [Finance].[TransportationView] Where Type ='{MoneyTransactionTypes.Transportation}'";
               
                transactions = new ObservableCollection<MoneyTransaction>(connection.Query<MoneyTransaction>(query));
            }

            viewData = new CollectionViewSource() { Source = transactions };
            viewData.Filter += DataFilter;

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
            var transaction = new MoneyTransaction()
            {
                Date = DateTime.Now,
                Type = MoneyTransactionTypes.Transportation.ToString()
            };
            TransactionWindow transactionWindow = new TransactionWindow()
            {
                UserData = UserData,
                ActionData = Actions.New,
                TransactionData = transaction,
                TransactionsData = transactions,
            };
            transactionWindow.ShowDialog();
        }

        private void Edit_ClicK(object sender, RoutedEventArgs e)
        {
            if (TransactionsList.SelectedItem is MoneyTransaction transaction)
            {
                TransactionWindow transactionWindow = new TransactionWindow()
                {
                    UserData = UserData,
                    ActionData = Actions.Edit,
                    TransactionData = transaction,
                    TransactionsData = null,
                };
                transactionWindow.ShowDialog();
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

        #region Filters

        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(MoneyTransaction).GetProperty("Code"),
            typeof(MoneyTransaction).GetProperty("Date"),
            typeof(MoneyTransaction).GetProperty("Amount"),
            typeof(MoneyTransaction).GetProperty("Description"),
            typeof(MoneyTransaction).GetProperty("AccountName"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is MoneyTransaction record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        else
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}".ToUpper();

                        if (!value.Contains(((TextBox)FindName(property.Name)).Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        private void ApplyFilter(object sender, KeyEventArgs e)
        {
            viewData.View.Refresh();
        }
        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            foreach (PropertyInfo property in filterProperties)
            {
                ((TextBox)FindName(property.Name)).Text = null;
            }
            viewData.View.Refresh();
        }

        #endregion

    }
}
