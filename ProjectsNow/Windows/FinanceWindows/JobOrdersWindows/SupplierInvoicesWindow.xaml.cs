using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.FinanceWindows.JobOrdersWindows
{
    public partial class SupplierInvoicesWindow : Window
    {
        public User UserData { get; set; }
        public JobOrderFinance JobOrderData { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<Database.Suppliers.SupplierInvoice> supplierInvoices;
        public SupplierInvoicesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Finance].[JobAnalysis(JobOrdersInvoices)] Where JobOrderID = {JobOrderData.ID} Order By InvoiceNumber";
                supplierInvoices = new ObservableCollection<Database.Suppliers.SupplierInvoice>(connection.Query<Database.Suppliers.SupplierInvoice>(query));
            }
            DataContext = new { UserData };

            foreach(Database.Suppliers.SupplierInvoice invoice in supplierInvoices)
            {
                if (invoice.SupplierID == 0)
                    invoice.SupplierName = DatabaseAI.FactoryStoreName;
            }
            viewData = new CollectionViewSource() { Source = supplierInvoices };

            InvoicesList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (supplierInvoices.Count == 0)
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
    }
}
