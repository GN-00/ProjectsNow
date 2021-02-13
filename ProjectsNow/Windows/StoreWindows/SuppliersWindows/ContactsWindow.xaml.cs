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
    public partial class ContactsWindow : Window
    {
        public User UserData { get; set; }
        public Supplier SupplierData { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<Database.Suppliers.Contact> contacts;
        public ContactsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"{DatabaseAI.GetFields<Database.Suppliers.Contact>()} Where SupplierID = {SupplierData.ID} Order By Name";
                contacts = new ObservableCollection<Database.Suppliers.Contact>(connection.Query<Database.Suppliers.Contact>(query));
            }

            viewData = new CollectionViewSource() { Source = contacts };

            viewData.Filter += DataFiltrer;

            ContactsList.ItemsSource = viewData.View;

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
            Database.Suppliers.Contact contact = new Database.Suppliers.Contact() { SupplierID = SupplierData.ID } ;
            ContactWindow contactWindow = new ContactWindow()
            {
                ActionData = Actions.New,
                ContactData = contact,
                ContactsData = contacts,
            };
            contactWindow.ShowDialog();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Database.Suppliers.Contact contact)
            {
                ContactWindow contactWindow = new ContactWindow()
                {
                    ActionData = Actions.Edit,
                    ContactData = contact,
                    ContactsData = contacts,
                };
                contactWindow.ShowDialog();
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedItem is Database.Suppliers.Contact contact)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserSupplierID(connection, contact.SupplierID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.SupplierID = contact.SupplierID;
                        UserController.UpdateSupplierID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Visibility.Visible;

                    MessageBoxResult result = CMessageBox.Show($"Deleting", $"Are you sure want to delete:\n{contact.Name} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            var checkSupplierUsage = connection.Query<int>($"Select SupplierID From [Purchase].[Orders] Where SupplierAttentionID = {contact.ID}");
                            if (checkSupplierUsage.Count() == 0 || checkSupplierUsage == null)
                            {
                                connection.Execute($"Delete From [Store].[SuppliersContacts] Where ID = {contact.ID} ");
                                contacts.Remove(contact);

                                UserData.SupplierID = null;
                                UserController.UpdateSupplierID(connection, UserData);
                            }
                            else
                            {
                                CMessageBox.Show("Deleting", $"You can't delete {contact.Name} data. \nIts used in projects data", CMessageBoxButton.OK, CMessageBoxImage.Warning);
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
            var selectedIndex = ContactsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Contacts: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ContactsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Contacts: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";

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
                    if (e.Item is Database.Suppliers.Contact contact)
                    {
                        string value = contact.Name.ToUpper();
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
            //using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            //{
            //    UserData.SupplierID = null;
            //    UserController.UpdateSupplierID(connection, UserData);
            //}
        }
    }
}
