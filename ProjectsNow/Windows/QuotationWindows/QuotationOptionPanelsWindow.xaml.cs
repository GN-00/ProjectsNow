using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationOptionPanelsWindow : Window
    {
        public int OptionID { get; set; }
        public int QuotationID { get; set; }
        public ObservableCollection<QuotationOptionPanel> OptionPanels { get; set; }


        CollectionViewSource viewData;
        ObservableCollection<QuotationOptionPanel> QPanels;
        public QuotationOptionPanelsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                QPanels = QuotationOptionPanelController.GetQuotationPanels(connection, QuotationID, OptionID);
            }

            viewData = new CollectionViewSource() { Source = QPanels };
            PanelsList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QuotationOptionPanel panelData)
            {
                panelData.OptionID = OptionID;

                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = DatabaseAI.InsertRecord<QuotationOptionPanel>();
                    panelData.ID = (int)(decimal)connection.ExecuteScalar(query, panelData);
                }

                QPanels.Remove(panelData);
                OptionPanels.Add(panelData);
            }
        }
    }
}
