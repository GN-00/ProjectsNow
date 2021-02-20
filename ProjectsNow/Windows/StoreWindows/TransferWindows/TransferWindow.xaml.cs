using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.StoreWindows.TransferWindows
{
    public partial class TransferWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }


        CollectionViewSource viewData;
        ObservableCollection<ItemTransaction> itemsData;
        IEnumerable<ItemPurchased> jobOrderItems;
        public TransferWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Store].[TransactionsView] Where JobOrderID = 0";
                itemsData = new ObservableCollection<ItemTransaction>(connection.Query<ItemTransaction>(query));

                query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] Where JobOrderID = {JobOrderData.ID}";
                jobOrderItems = connection.Query<ItemPurchased>(query);
                jobOrderItems = jobOrderItems.Where(i => i.RemainingQty > 0);
            }
            viewData = new CollectionViewSource() { Source = itemsData };
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
            if (ItemsList.SelectedItem is ItemTransaction item)
            {
                QtyWindow qtyWindow = new QtyWindow()
                {
                    ItemData = item,
                    InvoiceData = InvoiceData,
                    JobOrderData = JobOrderData,
                    ItemsData = ItemsData,
                    JobOrderItemData = jobOrderItems.FirstOrDefault(i => i.Code == item.Code),
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
                if (e.Item is ItemTransaction item)
                {
                    var checkJobOrderItem = jobOrderItems.FirstOrDefault(i => i.Code == item.Code);
                    if(checkJobOrderItem == null)
                    {
                        e.Accepted = false;
                        return;
                    }
                    if (item.FinalQty < 1)
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
