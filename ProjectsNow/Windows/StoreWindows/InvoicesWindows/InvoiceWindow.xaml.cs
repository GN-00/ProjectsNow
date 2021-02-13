using Dapper;
using System;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class InvoiceWindow : Window
    {
        public Actions ActionData { get; set; }
        public SupplierInvoice SupplierInvoiceData { get; set; }
        public ObservableCollection<SupplierInvoice> SupplierInvoicesData { get; set; }

        IEnumerable<Supplier> suppliers;
        SupplierInvoice supplierInvoice;
        public InvoiceWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            supplierInvoice = new SupplierInvoice();
            supplierInvoice.Update(SupplierInvoiceData);

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = "Select * From [Store].[Suppliers] Order By Name";
                suppliers = connection.Query<Supplier>(query);
            }
            SuppliersList.ItemsSource = suppliers;

            InvoiceDate.SelectedDate = supplierInvoice.Date;
            DataContext = supplierInvoice;
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

            if (supplierInvoice.SupplierName == null) { message += $"\n  Supplier Name."; isReady = false; }
            if (string.IsNullOrWhiteSpace(supplierInvoice.Number)) { message += $"\n  Number."; isReady = false; }
            
            if (!isReady)
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
                return;
            }

            if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"{DatabaseAI.UpdateRecord<SupplierInvoice>()}";
                    connection.Execute(query, supplierInvoice);
                }
                SupplierInvoiceData.Update(supplierInvoice);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"{DatabaseAI.InsertRecord<SupplierInvoice>()}";
                    supplierInvoice.ID = (int)(decimal)connection.ExecuteScalar(query, supplierInvoice);
                }
                SupplierInvoicesData.Add(supplierInvoice);
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SuppliersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SuppliersList.SelectedItem is Supplier supplier)
            {
                CodeTextBox.Text = supplierInvoice.SupplierCode = supplier.Code;
                supplierInvoice.SupplierName = supplier.Name;
            }
            else
            {
                CodeTextBox.Text = supplierInvoice.SupplierCode = null;
                supplierInvoice.SupplierName = null;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
                picker.SelectedDate = supplierInvoice.Date = DateTime.Today;
            else
                picker.SelectedDate = supplierInvoice.Date = date.Value;
        }
    }
}
