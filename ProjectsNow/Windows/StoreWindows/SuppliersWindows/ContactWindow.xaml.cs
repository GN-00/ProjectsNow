using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.SuppliersWindows
{
    public partial class ContactWindow : Window
    {
        public Actions ActionData { get; set; }
        public Database.Suppliers.Contact ContactData { get; set; }
        public ObservableCollection<Database.Suppliers.Contact> ContactsData { get; set; }

        Database.Suppliers.Contact contact;
        public ContactWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            contact = new Database.Suppliers.Contact();
            contact.Update(ContactData);

            ContactsList.ItemsSource = ContactsData;
            DataContext = contact;
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var checkName = ContactsData.Where(s => s.Name == ContactsList.Text);

            if ((checkName.Count() > 1 && ActionData == Actions.Edit) ||
                (checkName.Count() >= 1 && ActionData == Actions.New))
            {
                CMessageBox.Show("Name Error", "This contact name is already taken!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            if (ActionData == Actions.New)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.InsertRecord<Database.Suppliers.Contact>();
                    contact.ID = (int)(decimal)connection.ExecuteScalar(query, contact);
                }
                ContactsData.Add(contact);
            }
            else if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.UpdateRecord<Database.Suppliers.Contact>();
                    connection.Execute(query, contact);
                }
                ContactData.Update(contact);
            }

            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
