using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationPanelNameWindow : Window
    {
        public QPanel PanelData { get; set; }
        public ObservableCollection<QPanel> PanelsData { get; set; }


        QPanel newPanelData = new QPanel();
        public QuotationPanelNameWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newPanelData.Update(PanelData);

            DataContext = new { newPanelData };
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
            var checkName = PanelsData.Where(p => p.PanelName == newPanelData.PanelName).FirstOrDefault();

            if (checkName != null)
            {
                CMessageBox.Show("Name Error", $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            newPanelData.PanelSN = PanelsData.Max(p => p.PanelSN) + 1;

            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = DatabaseAI.InsertRecord<QPanel>();
                newPanelData.PanelID = (int)(decimal)connection.ExecuteScalar(query, newPanelData);

                List<QItem> items = QItemController.PanelItems(connection, PanelData.PanelID);

                if (items.Count != 0)
                {
                    var insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                 $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                 $"Values " +
                                 $"({newPanelData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                    connection.Execute(insert, items);
                }
            }

            PanelsData.Add(newPanelData);

            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
