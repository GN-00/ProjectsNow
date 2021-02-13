using Dapper;
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

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelItemsWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public List<Reference> ReferencesListData { get; set; }

        public Tables TableData;
        public ObservableCollection<JItem> ItemsData;
        public ObservableCollection<JItem> ItemsDetails;
        public ObservableCollection<JItem> ItemsEnclosure;

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
            PanelData.PanelDesignCost =
                ItemsDetails.Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                ItemsEnclosure.Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            double total = ItemsDetails.Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty) +
                           ItemsEnclosure.Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            double enclosureCost = ItemsEnclosure.Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<JItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            DetailsCost.Text = $"{detailsCost:N2} ({(detailsCost / total) * 100:N2} %)";
            EnclosureCost.Text = $"{enclosureCost:N2} ({(enclosureCost / total) * 100:N2} %)";
            CopperCost.Text = $"{copperCost:N2} ({(copperCost / total) * 100:N2} %)";
        }
        public PanelItemsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                ItemsDetails = JItemController.PanelDetails(connection, PanelData.PanelID);
                ItemsEnclosure = JItemController.PanelEnclosure(connection, PanelData.PanelID);
            }

            TableData = Tables.Details;
            ItemsData = ItemsDetails;

            viewData = new CollectionViewSource() { Source = ItemsData };
            viewData.Filter += DataFilter;
            ItemsList.ItemsSource = viewData.View;

            DataContext = new { UserData, PanelData, JobOrderData };
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
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
        private void Modification_Click(object sender, RoutedEventArgs e)
        {
            ItemsModificationWindow itemsModificationWindow = new ItemsModificationWindow()
            {
                UserData = this.UserData,
                JobOrderData = this.JobOrderData,
            };

            itemsModificationWindow.ShowDialog();
            Window_Loaded(sender, e);
        }
        private void ModificationsHistory_Click(object sender, RoutedEventArgs e)
        {
            ModificationsHistoryWindow modificationsHistoryWindow = new ModificationsHistoryWindow()
            {
                UserData = this.UserData,
                JobOrderData = this.JobOrderData,
            };
            modificationsHistoryWindow.ShowDialog();
        }
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if(ItemsData.Count > 1)
                {
                    int minIndex = ItemsData.Where(i => i.ItemQty != 0).Min(i => i.ItemSort);

                    if (itemData.ItemSort > minIndex)
                    {
                        int currentIndex = itemData.ItemSort;
                        JItem previousItem = ItemsData.FirstOrDefault(item => item.ItemSort == ItemsData.Where(i => i.ItemSort < currentIndex && i.ItemQty != 0).Max(i => i.ItemSort));
                        int previousIndex = previousItem.ItemSort;

                        itemData.ItemSort = previousIndex;
                        previousItem.ItemSort = currentIndex;

                        ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(previousItem));

                        string query = $"Update [JobOrder].[PanelsItems] Set ItemSort = {-1} Where PanelID = {itemData.PanelID} And ItemSort = {currentIndex} And ItemTable = '{TableData}'" +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {currentIndex} Where PanelID = {itemData.PanelID} And ItemSort = {previousIndex} And ItemTable = '{TableData}'; " +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {previousIndex} Where PanelID = {itemData.PanelID} And ItemSort = {-1} And ItemTable = '{TableData}'; ";

                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            connection.Execute(query);
                        }
                    }
                }
                
            }
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if(ItemsData.Count > 1)
                {
                    int maxIndex  = ItemsData.Where(i => i.ItemQty != 0).Max(i => i.ItemSort);

                    if (itemData.ItemSort < maxIndex)
                    {
                        int currentIndex = itemData.ItemSort;
                        JItem nextItem = ItemsData.FirstOrDefault(item => item.ItemSort == ItemsData.Where(i => i.ItemSort > currentIndex && i.ItemQty != 0).Min(i => i.ItemSort));
                        int nextIndex = nextItem.ItemSort;

                        itemData.ItemSort = nextIndex;
                        nextItem.ItemSort = currentIndex;

                        ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(nextItem));

                        string query = $"Update [JobOrder].[PanelsItems] Set ItemSort = {-1} Where PanelID = {itemData.PanelID} And ItemSort = {currentIndex} And ItemTable = '{TableData}'" +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {currentIndex} Where PanelID = {itemData.PanelID} And ItemSort = {nextIndex} And ItemTable = '{TableData}'; " +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {nextIndex} Where PanelID = {itemData.PanelID} And ItemSort = {-1} And ItemTable = '{TableData}'; ";

                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            connection.Execute(query);
                        }
                    }
                }
            }
        }
        private void Details_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Details;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsDetails;

            viewData.Source = ItemsData;
            ItemsList.ItemsSource = viewData.View;
        }
        private void Enclosure_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Enclosure;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsEnclosure;

            viewData.Source = ItemsData;
            ItemsList.ItemsSource = viewData.View;
        }
        private void TableChange_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if (TableData == Tables.Details)
                {
                    itemData.ItemTable = Tables.Enclosure.ToString();
                    ItemsDetails.Remove(itemData);
                    itemData.ItemSort = ItemsEnclosure.Max(i => i.ItemSort) + 1;
                    ItemsEnclosure.Add(itemData);
                }
                else
                {
                    itemData.ItemTable = Tables.Details.ToString();
                    ItemsEnclosure.Remove(itemData);
                    itemData.ItemSort = ItemsDetails.Max(i => i.ItemSort) + 1;
                    ItemsDetails.Add(itemData);
                }

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"Update [JobOrder].[PanelsItems] Set " +
                                   $"ItemTable = '{itemData.ItemTable}', ItemSort = {itemData.ItemSort} " +
                                   $"Where PanelID = {itemData.PanelID} And Code ='{itemData.Code}'";
                    connection.Execute(query, itemData);
                }
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(JItem).GetProperty("PartNumber"),
            typeof(JItem).GetProperty("Description"),
            typeof(JItem).GetProperty("ItemQty"),
            typeof(JItem).GetProperty("Brand"),
            typeof(JItem).GetProperty("ItemCost"),
            typeof(JItem).GetProperty("ItemPrice"),
            typeof(JItem).GetProperty("ItemTotalCost"),
            typeof(JItem).GetProperty("ItemTotalPrice"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JItem record)
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

                        if (record.ItemQty == 0)
                        {
                            e.Accepted = false;
                            return;
                        }

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
