using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Enums;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class QuotationPanelItemsWindow : Window
    {
        public Tables TableData;
        public ObservableCollection<QItem> ItemsData;
        public ObservableCollection<QItem> ItemsDetails;
        public ObservableCollection<QItem> ItemsEnclosure;

        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public Quotation QuotationData { get; set; }
        public QuotationPanelsWindow QuotationPanelsWindowData { get; set; }
        public List<Reference> ReferencesListData { get; set; }

        public void ListChanged()
        {
            ////Unit Cost
            ///double total = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
            //                ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            ////Unit Price
            //double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit/100)));
            //double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));
            //double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));

            //Total Price
            double total = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty) +
                           ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            DetailsCost.Text = $"{detailsCost:N2} ({(detailsCost / total) * 100:N2} %)";
            EnclosureCost.Text = $"{enclosureCost:N2} ({(enclosureCost / total) * 100:N2} %)";
            CopperCost.Text = $"{copperCost:N2} ({(copperCost / total) * 100:N2} %)"; ;
        }

        public QuotationPanelItemsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                ItemsDetails = QItemController.PanelDetails(connection, PanelData.PanelID);
                ItemsEnclosure = QItemController.PanelEnclosure(connection, PanelData.PanelID);
            }

            TableData = Tables.Details;
            ItemsData = ItemsDetails;

            viewData = new CollectionViewSource() { Source = ItemsData };
            viewData.Filter += DataFilter;

            ItemsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            DataContext = new { UserData, PanelData, QuotationData };

            PanelData.PanelCost =
                ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            ListChanged();
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
            if (QuotationPanelsWindowData != null)
            {
                QuotationPanelsWindowData.Visibility = Visibility.Visible;
                QuotationData.QuotationCost = Math.Round
                    (QuotationPanelsWindowData.panelsData.Sum<QPanel>(panel => panel.PanelCost * panel.PanelQty / (1 - panel.PanelProfit / 100)), 3);
            }
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Details;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsDetails;

            viewData.Source = ItemsData;
            viewData.Filter += DataFilter;

            ItemsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
        }
        private void Enclosure_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Enclosure;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsEnclosure;

            viewData.Source = ItemsData;
            viewData.Filter += DataFilter;

            ItemsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(QItem).GetProperty("Article1"),
            typeof(QItem).GetProperty("Article2"),
            typeof(QItem).GetProperty("PartNumber"),
            typeof(QItem).GetProperty("Description"),
            typeof(QItem).GetProperty("ItemQty"),
            typeof(QItem).GetProperty("Brand"),
            typeof(QItem).GetProperty("ItemDiscount"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is QItem record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        else
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}".ToUpper();


                        if (!value.Contains(((TextBox)FindName(property.Name)).Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }

        private void ApplyFilter(object sender, KeyEventArgs e)
        {
            viewData.View.Refresh();
        }

        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            foreach (PropertyInfo property in filterProperties)
            {
                ((TextBox)FindName(property.Name)).Text = null;
            }
            viewData.View.Refresh();
        }

        #endregion
    }
}
