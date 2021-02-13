using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using ProjectsNow.Windows.ReferencesWindows;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.MainWindows
{
    public partial class MainWindow : Window
    {
        //public int UserID = 0; /// need work
        public User UserData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 14;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //#region Test
            //using (SqlConnection connection = new SqlConnection(DatabaseAI.connectionString))
            //{
            //    UserData = connection.Query<User>($"Select * From [User].[Users] Where UserID = {UserID}").ToList().FirstOrDefault();
            //}
            //#endregion
            DataContext = UserData;

            var subWindow = new TendaringControl(this.UserData);
            Controls.Children.Add(subWindow);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Tendaring_Click(object sender, RoutedEventArgs e)
        {
            Controls.Children.Clear();
            Controls.Children.Add(new TendaringControl(this.UserData));
        }
        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            Controls.Children.Clear();
            Controls.Children.Add(new ProjectsControl() { UserData = this.UserData, MainWindowData = this });
        }

        private void Store_Click(object sender, RoutedEventArgs e)
        {
            Controls.Children.Clear();
            Controls.Children.Add(new ItemsControl() { UserData = this.UserData, MainWindowData = this });
        }
        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            Controls.Children.Clear();
            Controls.Children.Add(new FinanceControl() { UserData = this.UserData, MainWindowData = this });
        }
        private void UserInfo_Click(object sender, RoutedEventArgs e)
        {
            //UserWindow userWindow = new UserWindow() { UserData = this.UserData };
            //userWindow.ShowDialog();
        }

        private void ssss_Click(object sender, RoutedEventArgs e)
        {
            //Delivery delivery;
            ////OrderAcknowledgement orderAcknowledgement;
            //////Inquiry inquiry = new Inquiry() { InquiryID = 38 };
            //using (SqlConnection connection = new SqlConnection(DatabaseAI.connectionString))
            //{
            //    var jo = JobOrderController.GetJobOrder(connection, 5);
            //    var ss = TPanelController.Transactions(connection, 5, Statuses.Delivered).FirstOrDefault();
            //    var s = TPanelController.GetTPanels(connection, 5, Statuses.Delivered).ToList();
            //    var sss = Contact.GetProjectAttention(connection, jo.InquiryID);
            //    //var pu = PurchaseOrderController.QuotationPurchaseOrders(connection, jo.QuotationID).FirstOrDefault();
            //    //orderAcknowledgement = new OrderAcknowledgement(jo, pu);
            //    delivery = new Delivery(jo, sss, ss, s);
            //}

            //FrameworkElement f = delivery;
            //Printing.Print.PrintPreview(f);
        }

        private void References_Click(object sender, RoutedEventArgs e)
        {
            ReferencesWindow referencesWindow = new ReferencesWindow()
            {
                UserData = this.UserData,
            };
            referencesWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserController.ResetIDs(connection, UserData.UserID);
            }
            App.Current.Shutdown();
        }


    }
}
