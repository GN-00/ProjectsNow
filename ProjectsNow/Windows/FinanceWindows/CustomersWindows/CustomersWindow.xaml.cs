using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.FinanceWindows.CustomersWindows
{
    public partial class CustomersWindow : Window
    {
        public User UserData { get; set; }

        ObservableCollection<CustomerInformation> Customers; 
        public CustomersWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Finance].[CustomersBalance]";
                Customers = new ObservableCollection<CustomerInformation>(connection.Query<CustomerInformation>(query));
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = Customers };
            viewData.Filter += DataFilter;

            CustomersList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (Customers.Count == 0)
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
        private void Statement_ClicK(object sender, RoutedEventArgs e)
        {
            if(CustomersList.SelectedItem is CustomerInformation customer)
            {
                StatementWindow statementWindow = new StatementWindow()
                {
                    UserData = UserData,
                    CustomerInformation = customer,
                };
                statementWindow.ShowDialog();
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = CustomersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Customers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Customer: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void CustomersList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = CustomersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Customers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Customer: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }


        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(CustomerInformation).GetProperty("CustomerName"),
            typeof(CustomerInformation).GetProperty("Debit"),
            typeof(CustomerInformation).GetProperty("Credit"),
            typeof(CustomerInformation).GetProperty("BalanceView"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is CustomerInformation record)
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
