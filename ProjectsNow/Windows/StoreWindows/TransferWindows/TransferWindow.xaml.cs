using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Events;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using ProjectsNow.Database.Store;
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
