using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PurchaseOrderWindow : Window
    {
        public int JobOrderID { get; set; }
        public Actions ActionData { get; set; }
        public PurchaseOrder PurchaseOrderData { get; set; }
        public ObservableCollection<PurchaseOrder> PurchaseOrdersData { get; set; }

        PurchaseOrder newPurchaseOrder = new PurchaseOrder();
        public PurchaseOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.Edit)
                newPurchaseOrder.Update(PurchaseOrderData);

            DataContext = new { newPurchaseOrder };
        }
        private void POPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
            {
                picker.SelectedDate = DateTime.Now;
            }
            else
            {
                picker.SelectedDate = date.Value;
            }
        }
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var checkNumber = PurchaseOrdersData.Where(po => po.Number == newPurchaseOrder.Number).FirstOrDefault();
            if (checkNumber != null)
            {
                CMessageBox.Show("Number Error", $"P.O is already exist!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            bool isReady = true;
            string message = $"Please Enter:";
            if (string.IsNullOrWhiteSpace(PONumber.Text))
            {
                isReady = false;
                message += $"\nPurchase Order Number!";
            }

            if (isReady)
            {
                if (ActionData == Actions.New)
                {
                    newPurchaseOrder.JobOrderID = JobOrderID;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.InsertRecord<PurchaseOrder>();
                        newPurchaseOrder.ID = (int)(decimal)connection.ExecuteScalar(query, newPurchaseOrder);
                    }

                    PurchaseOrdersData.Add(newPurchaseOrder);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.UpdateRecord<PurchaseOrder>();
                        connection.Execute(query, newPurchaseOrder);
                    }

                    PurchaseOrderData.Update(newPurchaseOrder);
                }

                Close();
            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
            }
        }
    }
}
