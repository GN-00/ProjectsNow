using Dapper;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using System;

namespace ProjectsNow.Windows.FinanceWindows
{
    public partial class CompanyAccountWindow : Window
    {
        public User UserData { get; set; }
        public Actions ActionData { get; set; }
        public Account AccountData { get; set; }
        public ObservableCollection<Account> AccountsData { get; set; }

        Account newAccount;
        public CompanyAccountWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newAccount = new Account() { CreateDate = DateTime.Today };
            newAccount.Update(AccountData);

            DataContext = newAccount;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.AccountID = null;
                UserController.UpdateAccountID(connection, UserData);
            }
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newAccount.Name))
            {
                string query;
                if(ActionData == Actions.New)
                {
                    query = DatabaseAI.InsertRecord<Account>();
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        newAccount.ID = (int)(decimal)connection.ExecuteScalar(query, newAccount);
                    }

                    AccountsData.Add(newAccount);
                }
                else
                {
                    query = DatabaseAI.UpdateRecord<Account>();
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute(query, newAccount);
                    }

                    AccountData.Update(newAccount);
                }

                this.Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
