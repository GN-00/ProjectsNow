using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows
{
    public partial class CompanyAccountsWindow : Window
    {
        public User UserData { get; set; }

        ObservableCollection<Account> accounts;
        public CompanyAccountsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Finance].[CompanyAccountsBalancesView]";
                accounts = new ObservableCollection<Account>(connection.Query<Account>(query));
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = accounts };
            AccountsList.ItemsSource = viewData.View;

            viewData.Filter += DataFilter;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = AccountsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Accounts: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Account: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewData.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }
        private void AccountsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = AccountsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Accounts: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Account: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
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

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account() { CreateDate = DateTime.Today };
            CompanyAccountWindow companyAccountWindow = new CompanyAccountWindow()
            {
                ActionData = Actions.New,
                UserData = UserData,
                AccountData = account,
                AccountsData = accounts,
            };
            companyAccountWindow.ShowDialog();
        }
        private void Edit_ClicK(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem is Account account)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserAccountID(connection, account.ID);

                    if (usedBy == null)
                    {
                        UserData.AccountID = account.ID;
                        UserController.UpdateAccountID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    CompanyAccountWindow companyAccountWindow = new CompanyAccountWindow()
                    {
                        ActionData = Actions.Edit,
                        UserData = UserData,
                        AccountData = account,
                    };
                    companyAccountWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This account underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem is Account account)
            {
                Account checkAccount;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"Select AccountID As ID From [Finance].[MoneyTransactions] Where AccountID = {account.ID}";
                    checkAccount = connection.QueryFirstOrDefault<Account>(query);
                }

                if(checkAccount == null)
                {
                    MessageBoxResult result = CMessageBox.Show("Account", $"Are you sure want to delete\n{account.Name}?", CMessageBoxButton.YesNo, CMessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            string query = $"Delete From [Finance].[CompanyAccounts] Where ID = {account.ID};" +
                                           $"Delete From [Finance].[BankAccounts] Where ID = {account.BankID};";
                            connection.Execute(query);
                        }

                        accounts.Remove(account);
                    }
                }
                else
                {
                    CMessageBox.Show("Account", "Can't delete this account!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(Account).GetProperty("Name"),
            typeof(Account).GetProperty("Balance"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Account record)
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

                        TextBox textBox;
                        if (property.Name == "Name")
                            textBox = AccountName;
                        else
                            textBox = (TextBox)FindName(property.Name);

                        if (!value.Contains(textBox.Text.ToUpper()))
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

        private void Bank_Click(object sender, RoutedEventArgs e)
        {
            if(AccountsList.SelectedItem is Account account)
            {
                if(account.BankID != 0)
                {
                    BankAccount bankAccount;
                    using(SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Select * From [Finance].[BankAccounts] Where ID = {account.BankID}";
                        bankAccount = connection.QueryFirstOrDefault<BankAccount>(query);
                    }
                    BankInformationWindow bankInformationWindow = new BankInformationWindow()
                    {
                        BankData = bankAccount,
                    };
                    bankInformationWindow.ShowDialog();
                }
                else
                {
                    string query;
                    BankAccount bankAccount = new BankAccount();
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        query = $"{DatabaseAI.InsertRecord<BankAccount>()}";
                        bankAccount.ID = (int)(decimal)connection.ExecuteScalar(query, bankAccount);

                        account.BankID = bankAccount.ID;
                        query = DatabaseAI.UpdateRecord<Account>();
                        connection.Execute(query, account);
                    }
                    BankInformationWindow bankInformationWindow = new BankInformationWindow()
                    {
                        BankData = bankAccount,
                    };
                    bankInformationWindow.ShowDialog();
                }
            }
        }
    }
}
