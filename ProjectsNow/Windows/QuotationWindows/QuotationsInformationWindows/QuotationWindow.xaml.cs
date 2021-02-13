using System.Linq;
using System.Windows;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class QuotationWindow : Window
    {
        public User UserData { get; set; }
        public int QuotationID { get; set; }
        public bool OpenPanelsWindow { get; set; }
        public Quotation QuotationData { get; set; }

        CollectionViewSource viewProjectContacts;
        ObservableCollection<Contact> projectContacts;

        Customer customerData;
        Consultant consultantData;
        Quotation newQuotationData = new Quotation();

        public QuotationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (QuotationData == null)
                    QuotationData = QuotationController.GetQuotation(connection, QuotationID);

                newQuotationData.Update(QuotationData);

                projectContacts = ContactController.GetProjectContacts(connection, newQuotationData.InquiryID);
                viewProjectContacts = new CollectionViewSource { Source = projectContacts };
                ProjectContactsList.ItemsSource = viewProjectContacts.View;

                CustomerList.ItemsSource = CustomerController.GetCustomers(connection);

                SalesList.ItemsSource = SalesmanController.GetSalesmen(connection);

                EstimationList.ItemsSource = EstimationController.GetEstimation(connection);

                ConsultantList.ItemsSource = ConsultantController.GetConsultants(connection);
            }

            if (OpenPanelsWindow) Cancel.Visibility = Visibility.Collapsed;

            viewProjectContacts.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            DataContext = new { newQuotationData, customerData, consultantData };
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
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.QuotationID = null;
                UserController.UpdateQuotationID(connection, UserData);
            }
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow_Click(sender, e);
        }

        private void CustomerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customerData = (((ComboBox)sender).SelectedItem as Customer);
            DataContext = new { newQuotationData, customerData, consultantData };
        }
        private void Consultant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            consultantData = (((ComboBox)sender).SelectedItem as Consultant);
            DataContext = new { newQuotationData, customerData, consultantData };
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (projectContacts.Count != 0)
            {
                var selectedIndex = ProjectContactsList.SelectedIndex;
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewProjectContacts.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPanel.Text = $"Contact: 0";
            }
        }
        private void ProjectContactsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ProjectContactsList.SelectedIndex;
            NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewProjectContacts.View.Cast<object>().Count()}";
        }
    }
}
