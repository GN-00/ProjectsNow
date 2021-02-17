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
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows.SuppliersInvoicesWindows
{
    public partial class InvoicesWindow : Window
    {
        public User UserData { get; set; }
        public JobOrderFinance JobOrderData { get; set; }

        List<int> years;
        bool isLoading = true;
        ObservableCollection<Database.Suppliers.SupplierInvoice> supplierInvoices;
        public InvoicesWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Finance].[JobAnalysis(OriginalsInvoices)] Where Year(Date) = {DateTime.Now.Year} Order By InvoiceNumber, JobOrderID";
                supplierInvoices = new ObservableCollection<Database.Suppliers.SupplierInvoice>(connection.Query<Database.Suppliers.SupplierInvoice>(query));

                query = $"Select * From [Finance].[JobAnalysis(OriginalsInvoicesYear))]";
                years = connection.Query<int>(query).ToList();
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = supplierInvoices };
            viewData.Filter += DataFilter;

            YearsList.ItemsSource = years;
            InvoicesList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (supplierInvoices.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            isLoading = false;
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
            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Invoices: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Invoice: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void InvoicesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Invoices: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Invoice: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (YearsList.SelectedItem is int year)
            {
                string query;
                DeleteFilter_Click(sender, e);
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Finance].[JobAnalysis(SuppliersInvoices)] Where Year(Date) = {year}";
                    supplierInvoices = new ObservableCollection<Database.Suppliers.SupplierInvoice>(connection.Query<Database.Suppliers.SupplierInvoice>(query));
                }
                InvoicesList.ItemsSource = viewData.View;
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("Code"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("InvoiceNumber"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("SupplierName"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("InvoiceTotal"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("Paid"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("BalanceView"),
            typeof(Database.Suppliers.SupplierInvoice).GetProperty("Status"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Database.Suppliers.SupplierInvoice record)
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

        private void PostingItems_ClicK(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Database.Suppliers.SupplierInvoice supplierInvoice)
            {
                string query;
                JobOrder jobOrderData;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[JobOrdersInformation] Where ID = {supplierInvoice.JobOrderID}";
                    jobOrderData = connection.QueryFirstOrDefault<JobOrder>(query);
                }
                StoreWindows.InvoicesWindows.InvoicesWindow invoicesWindow = new StoreWindows.InvoicesWindows.InvoicesWindow()
                {
                    UserData = UserData,
                    JobOrderData = jobOrderData,
                };
                invoicesWindow.ShowDialog();
            }
        }

        private void Invoicing_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Database.Suppliers.SupplierInvoice supplierInvoice)
            {
                User usedBy;
                string query;
                JobOrder jobOrderData = new JobOrder();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, supplierInvoice.JobOrderID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderID = supplierInvoice.JobOrderID;
                        UserController.UpdateJobOrderID(connection, UserData);

                        query = $"Select * From [JobOrder].[JobOrdersInformation] Where ID = {supplierInvoice.JobOrderID}";
                        jobOrderData = connection.QueryFirstOrDefault<JobOrder>(query);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var window = new JobOrderWindows.Panels_Invoicing_Windows.InvoicingWindow()
                    {
                        UserData = UserData,
                        PanelsData = null,
                        JobOrderData = jobOrderData,
                    };
                    window.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Transactions_ClicK(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Database.Suppliers.SupplierInvoice supplierInvoice)
            {
                InvoiceWindow invoiceWindow = new InvoiceWindow()
                {
                    UserData = UserData,
                    SupplierInvoice= supplierInvoice,
                };
                invoiceWindow.ShowDialog();
            }
        }
    }
}
