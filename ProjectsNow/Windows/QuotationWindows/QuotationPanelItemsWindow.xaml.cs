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
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.ReferencesWindows;

namespace ProjectsNow.Windows.QuotationWindows
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
            double total = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                           ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            ////double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            ////double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            ////double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            //Unit Price
            double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            ////Unit Price
            //double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));
            //double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));
            //double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));

            //Total Price
            //PanelData.PanelCost =
            //    ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
            //    ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            //double total = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty) +
            //               ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            //double detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            //double enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);
            //double copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)) * PanelData.PanelQty);

            DetailsCost.Text = $"{detailsCost:N2} ({(detailsCost / total) * 100:N2} %)";
            EnclosureCost.Text = $"{enclosureCost:N2} ({(enclosureCost / total) * 100:N2} %)";
            CopperCost.Text = $"{copperCost:N2} ({(copperCost / total) * 100:N2} %)";
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
            
            ItemsDetails.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            ItemsEnclosure.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            PanelData.PanelCost =
                ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            DataContext = new { UserData, PanelData, QuotationData };

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
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListChanged();
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemType.Text == "Standard Item")
            {
                ItemWindow itemWindow = new ItemWindow()
                {
                    SelectedIndex = this.ItemsData.Count,
                    ActionData = Actions.New,
                    UserData = this.UserData,
                    PanelItemsWindowData = this,
                    ReferencesListData = this.ReferencesListData,
                };
                itemWindow.ShowDialog();
            }
            else
            {
                NewItemWindow newItemWindow = new NewItemWindow()
                {
                    UserData = this.UserData,
                    ActionData = Actions.New,
                    SelectedIndex = this.ItemsData.Count,
                    PanelItemsWindowData = this,
                    ReferencesListData = this.ReferencesListData,
                };
                newItemWindow.ShowDialog();
            }
        }
        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                if (itemData.ItemType == ItemTypes.NewItem.ToString())
                {
                    NewItemWindow newItemWindow = new NewItemWindow()
                    {
                        ActionData = Actions.Edit,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ItemData = itemData,
                        ReferencesListData = this.ReferencesListData,
                    };
                    newItemWindow.ShowDialog();
                }
                else
                {
                    ItemWindow itemWindow = new ItemWindow()
                    {
                        ActionData = Actions.Edit,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ItemData = itemData,
                        ReferencesListData = this.ReferencesListData,
                    };
                    itemWindow.ShowDialog();
                }
            }
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                QItem newItem = new QItem();
                newItem.Update(itemData);
                int index;

                if (TableData == Tables.Details)
                    index = ItemsDetails.Count;
                else
                    index = ItemsEnclosure.Count;

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    newItem.ItemSort = index;
                    string query = DatabaseAI.InsertRecord<QItem>();
                    newItem.ItemID = (int)(decimal)connection.ExecuteScalar(query, newItem);
                }

                if (TableData == Tables.Details)
                    ItemsDetails.Add(newItem);
                else
                    ItemsEnclosure.Add(newItem);

                PanelData.PanelCost =
                        ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                        ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

                ListChanged();
            }
        }
        private void InsertUp_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                if (ItemType.Text == "Standard Item")
                {
                    ItemWindow itemWindow = new ItemWindow()
                    {
                        SelectedIndex = this.ItemsData.IndexOf(itemData),
                        ActionData = Actions.InsertUp,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ReferencesListData = this.ReferencesListData,
                    };
                    itemWindow.ShowDialog();
                }
                else
                {
                    NewItemWindow newItemWindow = new NewItemWindow()
                    {
                        SelectedIndex = this.ItemsData.IndexOf(itemData),
                        ActionData = Actions.InsertUp,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ReferencesListData = this.ReferencesListData,
                    };
                    newItemWindow.ShowDialog();
                }
            }
        }
        private void InsertDown_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                if (ItemType.Text == "Standard Item")
                {
                    ItemWindow itemWindow = new ItemWindow()
                    {
                        SelectedIndex = this.ItemsData.IndexOf(itemData) + 1,
                        ActionData = Actions.InsertDown,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ReferencesListData = this.ReferencesListData,
                    };
                    itemWindow.ShowDialog();
                }
                else
                {
                    NewItemWindow newItemWindow = new NewItemWindow()
                    {
                        SelectedIndex = this.ItemsData.IndexOf(itemData) + 1,
                        ActionData = Actions.InsertDown,
                        UserData = this.UserData,
                        PanelItemsWindowData = this,
                        ReferencesListData = this.ReferencesListData,
                    };
                    newItemWindow.ShowDialog();
                }
            }
        }
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                if (itemData.ItemSort > 0)
                {
                    itemData.ItemSort -= 1;
                    ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(itemData) - 1);
                    foreach (QItem item in ItemsData.Where(item => item.ItemSort == itemData.ItemSort && item.ItemID != itemData.ItemID))
                    {
                        ++item.ItemSort;
                    }
                    string query = $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort + 1 Where ItemSort = {itemData.ItemSort} And PanelID = {itemData.PanelID} And ItemID != {itemData.ItemID} And ItemTable = '{TableData}'; " +
                                   $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort - 1 Where ItemID = {itemData.ItemID}; ";
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute(query);
                    }
                }
            }
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                if (itemData.ItemSort < ItemsData.Count && ItemsData.Count != 1)
                {
                    itemData.ItemSort += 1;
                    ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(itemData) + 1);
                    foreach (QItem item in ItemsData.Where(item => item.ItemSort == itemData.ItemSort && item.ItemID != itemData.ItemID))
                    {
                        ++item.ItemSort;
                    }
                    string query = $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort - 1 Where ItemSort = {itemData.ItemSort} And PanelID = {itemData.PanelID} And ItemID != {itemData.ItemID} And ItemTable = '{TableData}'; " +
                                   $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort + 1 Where ItemID = {itemData.ItemID}; ";
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute(query);
                    }
                }
            }
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QItem itemData)
            {
                MessageBoxResult result = CMessageBox.Show("Deleting", $"Are you sure you want to \ndelete {itemData.PartNumber} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Delete From [Quotation].[QuotationsPanelsItems] Where ItemID = {itemData.ItemID}; " +
                                       $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort - 1 Where ItemSort > {itemData.ItemSort} And PanelID = {itemData.PanelID} And ItemTable = '{TableData}'; ";
                        connection.Execute(query);
                    }

                    foreach (QItem item in ItemsData.Where(i => i.ItemSort > itemData.ItemSort))
                    {
                        --item.ItemSort;
                    }

                    ItemsData.Remove(itemData);

                    PanelData.PanelCost =
                        ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                        ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
                }
            }
        }
        private void GroupDown_Click(object sender, RoutedEventArgs e)
        {

        }
        private void GroupUp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.N)
            {
                AddItem_Click(sender, e);
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
            if (ItemsList.SelectedItem is QItem itemData)
            {
                foreach (QItem item in ItemsData.Where(i => i.ItemSort > itemData.ItemSort))
                {
                    --item.ItemSort;
                }

                if (TableData == Tables.Details)
                {
                    itemData.ItemTable = Tables.Enclosure.ToString();
                    ItemsDetails.Remove(itemData);
                    itemData.ItemSort = ItemsEnclosure.Count;
                    ItemsEnclosure.Add(itemData);
                }
                else
                {
                    itemData.ItemTable = Tables.Details.ToString();
                    ItemsEnclosure.Remove(itemData);
                    itemData.ItemSort = ItemsDetails.Count;
                    ItemsDetails.Add(itemData);
                }

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = DatabaseAI.UpdateRecord<QItem>();
                    connection.Execute(query, itemData);

                    query = $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort - 1 Where ItemSort > {itemData.ItemSort} And PanelID = {itemData.PanelID} And ItemTable = '{TableData}'; ";
                    connection.Execute(query);
                }
            }
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

        private void SmartEnclosure_Click(object sender, RoutedEventArgs e)
        {
            EnclosuresWindow enclosuresWindow = new EnclosuresWindow()
            {
                UserData = this.UserData,
                PanelData = this.PanelData,
                ItemsDetails = this.ItemsDetails,
                ItemsEnclosure = this.ItemsEnclosure,
            };
            enclosuresWindow.ShowDialog();
        }
        private void DeleteSmartEnclosure_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = CMessageBox.Show("Delete", "Are you sure you want to delete the enclosure!!", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                List<QItem> items = new List<QItem>();
                items.AddRange(ItemsDetails.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString()));
                items.AddRange(ItemsEnclosure.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString()));
                int detailsCount = items.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                int enclosureCount = items.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();
                
                int itemsCounter = items.Count;
                if(itemsCounter != 0)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query;
                        query = $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {PanelData.PanelID} And SelectionGroup = '{SelectionGroups.SmartEnclosure}'";
                        query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {detailsCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Details}'); ";
                        query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {enclosureCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Enclosure}'); ";
                        connection.Execute(query);

                        PanelData.EnclosureType = null;
                        PanelData.EnclosureHeight = null;
                        PanelData.EnclosureWidth = null;
                        PanelData.EnclosureDepth = null;

                        PanelData.EnclosureMetalType = null;
                        PanelData.EnclosureColor = null;
                        PanelData.EnclosureIP = null;
                        PanelData.EnclosureForm = null;
                        PanelData.EnclosureLocation = null;
                        PanelData.EnclosureInstallation = null;
                        PanelData.EnclosureFunctional = null;

                        PanelData.EnclosureName = null;
                        query = DatabaseAI.UpdateRecord<QPanel>();
                        connection.Execute(query, PanelData);
                    }
                    
                    foreach(QItem item in items)
                    {
                        if (item.ItemTable == Tables.Details.ToString())
                            ItemsDetails.Remove(item);
                        else
                            ItemsEnclosure.Remove(item);
                    }

                    foreach (QItem item in ItemsDetails)
                    {
                        item.ItemSort -= detailsCount;
                    }

                    foreach (QItem item in ItemsEnclosure)
                    {
                        item.ItemSort -= enclosureCount;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (QuotationPanelsWindowData != null)
            {
                PanelData.PanelCost = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                                      ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

                QuotationPanelsWindowData.Visibility = Visibility.Visible;
                QuotationData.QuotationCost = Math.Round
                    (QuotationPanelsWindowData.panelsData.Sum<QPanel>(panel => panel.PanelCost * panel.PanelQty / (1 - panel.PanelProfit / 100)), 3);
            }
        }
    }
}
