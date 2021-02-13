using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.StockWindows
{
    public partial class JobOrderSelectorWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public StockWindow StockWindowData { get; set; }

        ObservableCollection<JobOrder> jobOrders;
        public JobOrderSelectorWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = "Select * From [Store].[JobOrders(HaveStock)]";
                jobOrders = new ObservableCollection<JobOrder>(connection.Query<JobOrder>(query));
                jobOrders.Insert(0, DatabaseAI.store);
            }

            JobOrdersList.ItemsSource = jobOrders;
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
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrder)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, jobOrder.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderID = jobOrder.ID;
                        UserController.UpdateJobOrderID(connection, UserData);
                    }
                }
                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    JobOrderData.Update(jobOrder);
                    this.Close();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void JO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Orders_Click(sender, e);
        }
    }
}
