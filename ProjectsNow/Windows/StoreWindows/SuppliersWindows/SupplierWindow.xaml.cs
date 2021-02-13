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
    public partial class SupplierWindow : Window
    {
        public Actions ActionData { get; set; }
        public Supplier SupplierData { get; set; }
        public ObservableCollection<Supplier> SuppliersData { get; set; }

        Supplier supplier;
        public SupplierWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            supplier = new Supplier();
            supplier.Update(SupplierData);

            SuppliersList.ItemsSource = SuppliersData;
            DataContext = supplier;
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
            var checkName = SuppliersData.Where(s => s.Name == SuppliersList.Text);
            
            if ((checkName.Count() > 1 && ActionData == Actions.Edit) ||
                (checkName.Count() >= 1 && ActionData == Actions.New))
            {
                CMessageBox.Show("Name Error", "This supplier name is already taken!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            var checkCode = SuppliersData.Where(s => s.Code == Code.Text);

            if ((checkCode.Count() > 1 && ActionData == Actions.Edit) ||
                (checkCode.Count() >= 1 && ActionData == Actions.New))
            {
                CMessageBox.Show("Code Error", "This supplier code is already taken!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            if (ActionData == Actions.New)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.InsertRecord<Supplier>();
                    supplier.ID = (int)(decimal)connection.ExecuteScalar(query, supplier);
                }
                SuppliersData.Add(supplier);
            }
            else if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.UpdateRecord<Supplier>();
                    connection.Execute(query, supplier);
                }
                SupplierData.Update(supplier);
            }

            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
