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

namespace ProjectsNow.Windows.JobOrderWindows.ItemPurchaseOrdersWindows
{
    public partial class ItemsWindow : Window
    {
        public CompanyPO CompanyPOData { get; set; }
        public ObservableCollection<CompanyPOTransaction> ItemsData { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<ItemPurchased> jobOrderItems;
        public ItemsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] Where JobOrderID = {CompanyPOData.JobOrderID}";
                jobOrderItems = new ObservableCollection<ItemPurchased>(connection.Query<ItemPurchased>(query));
            }
            viewData = new CollectionViewSource() { Source = jobOrderItems };
            viewData.Filter += DataFilter;

            ItemsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
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
            if (ItemsList.SelectedItem is ItemPurchased item)
            {
                QtyWindow qtyWindow = new QtyWindow()
                {
                    ActionData = Enums.Actions.New,
                    ItemData = item,
                    CompanyPOData = CompanyPOData,
                    ItemsData = ItemsData,
                };
                qtyWindow.ShowDialog();

                viewData.View.Refresh();
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ItemPurchased item)
                {
                    if (item.PurchasedQty + item.InOrderQty >= item.Qty)
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
        #endregion
    }
}
