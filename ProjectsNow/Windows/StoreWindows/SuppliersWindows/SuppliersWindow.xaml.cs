using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.ComponentModel;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.SuppliersWindows
{
    public partial class SuppliersWindow : Window
    {
        public User UserData { get; set; }
        public ObservableCollection<Supplier> SuppliersData { get; set; }

        CollectionViewSource viewData;
        public SuppliersWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (SuppliersData == null)
                {
                    string query = $"{DatabaseAI.GetFields<Supplier>()} Order By Name";
                    SuppliersData = new ObservableCollection<Supplier>(connection.Query<Supplier>(query));
                }
            }
            
            viewData = new CollectionViewSource() { Source = SuppliersData };

            viewData.Filter += DataFiltrer;
            
            SuppliersList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
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
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Supplier supplier = new Supplier();
            SupplierWindow supplierWindow = new SupplierWindow()
            {
                ActionData = Actions.New,
                SupplierData = supplier,
                SuppliersData = SuppliersData,
            };
            supplierWindow.ShowDialog();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if(SuppliersList.SelectedItem is Supplier supplier)
            {
                SupplierWindow supplierWindow = new SupplierWindow()
                {
                    ActionData = Actions.Edit,
                    SupplierData = supplier,
                    SuppliersData = SuppliersData,
                };
                supplierWindow.ShowDialog();
                viewData.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersList.SelectedItem is Supplier supplier)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserSupplierID(connection, supplier.ID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.SupplierID = supplier.ID;
                        UserController.UpdateSupplierID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Visibility.Visible;

                    MessageBoxResult result = CMessageBox.Show($"Deleting", $"Are you sure want to delete:\n{supplier.Name} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            var checkSupplierUsage = connection.Query<int>($"Select SupplierID From [Store].[Invoices] Where SupplierID = {supplier.ID}");
                            if (checkSupplierUsage.Count() == 0 || checkSupplierUsage == null)
                            {
                                connection.Execute($"Delete From [Store].[Suppliers] Where ID = {supplier.ID} ");
                                SuppliersData.Remove(supplier);

                                UserData.SupplierID = null;
                                UserController.UpdateSupplierID(connection, UserData);
                            }
                            else
                            {
                                CMessageBox.Show("Deleting", $"You can't delete {supplier.Name} data. \nIts used in projects data", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                            }
                        }
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            UserData.SupplierID = null;
                            UserController.UpdateSupplierID(connection, UserData);
                        }
                    }

                    LoadingControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CMessageBox.Show($"Access", $"Can't delete this supplier!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void List_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = SuppliersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Suppliers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Supplier: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = SuppliersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Suppliers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Supplier: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewData.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }
        private void DataFiltrer(object sender, FilterEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FindRecord.Text))
            {
                try
                {
                    e.Accepted = true;
                    if (e.Item is Supplier supplier)
                    {
                        string value = supplier.Name.ToUpper();
                        if (!value.Contains(FindRecord.Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
                catch
                {
                    e.Accepted = true;
                }
            }
        }
        private void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            viewData.View.Refresh();
        }
        private void RemoveFilter(object sender, RoutedEventArgs e)
        {
            FindRecord.Text = null;
            viewData.View.Refresh();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.SupplierID = null;
                UserController.UpdateSupplierID(connection, UserData);
            }
        }
        private void Contacts_Click(object sender, RoutedEventArgs e)
        {
            if(SuppliersList.SelectedItem is Supplier supplier)
            {
                ContactsWindow contactsWindow = new ContactsWindow()
                {
                    UserData = UserData,
                    SupplierData = supplier,
                };
                contactsWindow.ShowDialog();
            }
        }
    }
}
