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


namespace ProjectsNow.Windows.JobOrderWindows.ItemPurchaseOrdersWindows
{
    public partial class OrderWindow : Window
    {
        public Actions ActionData { get; set; }
        public CompanyPO OrderData { get; set; }
        public ObservableCollection<CompanyPO> OrdersData { get; set; }

        CompanyPO orderData;
        IEnumerable<Supplier> suppliers;
        public OrderWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            orderData = new CompanyPO();
            orderData.Update(OrderData);

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                int numberPO;
                string query;
                query = "Select * From [Store].[Suppliers] Order By Name";
                suppliers = connection.Query<Supplier>(query);

                query = $"Select * From [Purchase].[OrderNumber] Where Year = {DateTime.Today.Year}";
                numberPO = connection.QueryFirstOrDefault<int>(query);
                orderData.Number = ++numberPO;

                VATList.ItemsSource = connection.Query("Select VAT From [Purchase].[Orders] Where VAT Is Not Null Group By VAT");
                DeliverToPlaceList.ItemsSource = connection.Query("Select DeliverToPlace From [Purchase].[Orders] Where DeliverToPlace Is Not Null Group By DeliverToPlace");
                DeliveryAddressList.ItemsSource = connection.Query("Select DeliveryAddress From [Purchase].[Orders] Where DeliveryAddress Is Not Null Group By DeliveryAddress");
                DeliverToPersonList.ItemsSource = connection.Query("Select DeliverToPerson From [Purchase].[Orders] Where DeliverToPerson Is Not Null Group By DeliverToPerson");
                PaymentList.ItemsSource = connection.Query("Select Payment From [Purchase].[Orders] Where Payment Is Not Null Group By Payment");

                if (ActionData == Actions.New)
                {
                    if (orderData.Number > 99)
                        orderData.Code = $"{(DateTime.Today.Year - DatabaseAI.CompanyCreationYear) * 1000 + orderData.Number}/ER-PCAPS/{DateTime.Today.Month:00}/{DateTime.Today.Year}";
                    else
                        orderData.Code = $"{(DateTime.Today.Year - DatabaseAI.CompanyCreationYear) * 100 + orderData.Number}/ER-PCAPS/{DateTime.Today.Month:00}/{DateTime.Today.Year}";
                }
            }
            SuppliersList.ItemsSource = suppliers;

            InvoiceDate.SelectedDate = orderData.Date;
            DataContext = orderData;
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

            if (orderData.SupplierName == null) { message += $"\n  Supplier Name."; isReady = false; }
            if (!isReady)
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
                return;
            }

            if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"{DatabaseAI.UpdateRecord<CompanyPO>()}";
                    connection.Execute(query, orderData);
                }
                OrderData.Update(orderData);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"{DatabaseAI.InsertRecord<CompanyPO>()}";
                    orderData.ID = (int)(decimal)connection.ExecuteScalar(query, orderData);
                }
                OrdersData.Add(orderData);
            }

            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SuppliersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuppliersList.SelectedItem is Supplier supplier)
            {
                CodeTextBox.Text = orderData.SupplierCode = supplier.Code;
                orderData.SupplierName = supplier.Name;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    SuppliersAttentionsList.ItemsSource = connection.Query($"Select * From [Store].[SuppliersContacts] Where SupplierID = {supplier.ID} Order By Name ");
                }
            }
            else
            {
                CodeTextBox.Text = orderData.SupplierCode = null;
                orderData.SupplierName = null;
                SuppliersAttentionsList.ItemsSource = null;
            }
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
                picker.SelectedDate = orderData.Date = DateTime.Today;
            else
                picker.SelectedDate = orderData.Date = date.Value;
        }
    }
}
