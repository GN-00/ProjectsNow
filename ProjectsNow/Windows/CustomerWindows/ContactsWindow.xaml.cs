using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.CustomerWindows
{
    public partial class ContactsWindow : Window
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int InquiryID { get; set; }
        public User UserData { get; set; }


        public ComboBox ComboBoxToUpdate { get; set; }
        public DataGrid DataGridToUpadate { get; set; }
        public ObservableCollection<Contact> RecordsData { get; set; }

        Actions action;
        Contact oldData;
        Contact recordData;
        List<int> contactsIDs;


        CollectionViewSource viewRecordsData;
        public ContactsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                RecordsData = new ObservableCollection<Contact>(ContactController.GetCustomerContacts(connection, CustomerID));
            }

            viewRecordsData = new CollectionViewSource() { Source = RecordsData };
            RecordsList.ItemsSource = viewRecordsData.View;

            viewRecordsData.Filter += DataFiltrer;
            viewRecordsData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            recordData = RecordsList.SelectedItem as Contact;
            DataContext = new { recordData, UserData };
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
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Visible;
            Done.Visibility = ReadOnly.Visibility = Visibility.Collapsed;
            action = Actions.New;
            recordData = new Contact()
            {
                CustomerID = this.CustomerID,
                CustomerName = this.CustomerName,
            };
            DataContext = new { recordData, UserData };
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            User usedBy;
            if (recordData != null)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserContactID(connection, recordData.ContactID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.ContactID = recordData.ContactID;
                        UserController.UpdateContactID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Visible;
                    Done.Visibility = ReadOnly.Visibility = Visibility.Collapsed;
                    action = Actions.Edit;
                    oldData = new Contact();
                    oldData.Update(recordData);
                }
                else
                {
                    CMessageBox.Show($"Access", $"This contact data underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            User usedBy;
            if (recordData != null)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserContactID(connection, recordData.ContactID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.ContactID = recordData.ContactID;
                        UserController.UpdateContactID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Visibility.Visible;

                    MessageBoxResult result = CMessageBox.Show($"Deleting", $"Are you sure want to delete:\n{recordData.ContactName} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            if (recordData.AbilityToDelete(connection))
                            {
                                connection.Execute($"Delete From [Customer].[Contacts] Where ContactID = {recordData.ContactID} ");
                                UpdateOtherWindow(connection);
                                RecordsData.Remove(recordData);

                                UserData.ContactID = null;
                                UserController.UpdateContactID(connection, UserData);
                            }
                            else
                            {
                                CMessageBox.Show("Deleting", $"You can't delete {recordData.ContactName} data \n Because its used in projects data", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                            }
                        }
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            UserData.ContactID = null;
                            UserController.UpdateContactID(connection, UserData);
                        }
                    }

                    LoadingControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CMessageBox.Show($"Access", $"This contact data underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Collapsed;
            Done.Visibility = ReadOnly.Visibility = Visibility.Visible;
            if (action == Actions.New)
            {
                List<Contact> contacts = RecordsData.ToList();
                contacts.Add(recordData);
                contacts.Sort((x, y) => x.ContactName.CompareTo(y.ContactName));
                int index = contacts.IndexOf(recordData);
                RecordsData.Insert(index, recordData);
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.InsertRecord<Contact>();
                    recordData.ContactID = (int)(decimal)connection.ExecuteScalar(query, recordData);

                    UpdateOtherWindow(connection);
                }
            }
            else if (action == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.UpdateRecord<Contact>();
                    connection.Execute(query, recordData);

                    UpdateOtherWindow(connection);

                    UserData.ContactID = null;
                    UserController.UpdateContactID(connection, UserData);
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Collapsed;
            Done.Visibility = ReadOnly.Visibility = Visibility.Visible;
            if (action == Actions.New)
            {
                recordData = RecordsList.SelectedItem as Contact;
                DataContext = new { recordData, UserData };
            }
            else if (action == Actions.Edit)
            {
                recordData.Update(oldData);
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    UserData.ContactID = null;
                    UserController.UpdateContactID(connection, UserData);
                }
            }
        }
        private void List_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            recordData = RecordsList.SelectedItem as Contact;
            var selectedIndex = RecordsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Contacts: {viewRecordsData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewRecordsData.View.Cast<object>().Count()}";
            DataContext = new { recordData, UserData };
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = RecordsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Contacts: {viewRecordsData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewRecordsData.View.Cast<object>().Count()}";
        }
        private void UpdateOtherWindow(SqlConnection connection)
        {
            if (DataGridToUpadate != null && InquiryID != 0)
            {
                DataGridToUpadate.ItemsSource = ContactController.GetProjectContacts(connection, InquiryID);
                contactsIDs = new List<int>();
                foreach (Contact contact in DataGridToUpadate.ItemsSource)
                {
                    contactsIDs.Add(contact.ContactID);
                }
                ComboBoxToUpdate.ItemsSource = ContactController.GetCustomerRemainingContacts(connection, recordData.CustomerID, contactsIDs.ToArray());
            }
        }


        private void DataFiltrer(object sender, FilterEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FindRecord.Text))
            {
                try
                {
                    e.Accepted = true;
                    if (e.Item is Contact contact)
                    {
                        string value = contact.ContactName.ToUpper();
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
            viewRecordsData.View.Refresh();
        }

        private void RemoveFilter(object sender, RoutedEventArgs e)
        {
            FindRecord.Text = null;
            viewRecordsData.View.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.ContactID = null;
                UserController.UpdateContactID(connection, UserData);
            }
        }
    }
}
