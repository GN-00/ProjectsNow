using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Reflection;
using ProjectsNow.Events;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class ItemsModificationWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        List<MItem> jobOrderItems;
        List<Reference> referencesData;
        List<Modification> modifications;
        ObservableCollection<MItem> items;
        ObservableCollection<MPanel> panels;
        public ItemsModificationWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (JobOrderData == null)
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);

                panels = MPanelController.GetPanels(connection, JobOrderData.ID);
                items = MItemController.GetModificationsItems(connection, JobOrderData.ID);
                jobOrderItems = MItemController.GetAllItems(connection, JobOrderData.ID);
                referencesData = ReferenceController.GetReferences(connection);
            }

            viewDataPanels = new CollectionViewSource() { Source = panels };
            viewDataItems = new CollectionViewSource() { Source = items };

            viewDataItems.Filter += DataFilter;

            PanelsList.ItemsSource = viewDataPanels.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Panels_CollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (viewDataPanels.View.Cast<object>().Count() == 0)
                Panels_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewDataItems.View.Cast<object>().Count() == 0)
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            modifications = items.GroupBy(i => i.ModificationID, ii => ii.Date).Select(m => new Modification() { ID = m.Key, Date = m.ToList()[0]}).OrderByDescending(m => m.ID).ToList();
            ModificationsList.ItemsSource = modifications;
            ModificationsList.SelectedItem = modifications.FirstOrDefault();

            DataContext = new { JobOrderData, UserData };
        }
        private void Panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataPanels.View.SortDescriptions.Add(new SortDescription("ItemSort", ListSortDirection.Ascending));
            }
        }

        #region Filters

        CollectionViewSource viewDataPanels;
        CollectionViewSource viewDataItems;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(MPanel).GetProperty("PanelID"),
        };

        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is MItem record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        else
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";

                        if (PanelsList.SelectedItem is MPanel panel)
                            if (value != panel.PanelID.ToString())
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

        #endregion
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Visible;
            ControlLock2.Visibility = Visibility.Collapsed;
            int newID = 0;
            if (modifications.Count != 0)
                newID = (modifications.Max<Modification>(m => m.ID));

            var newModification = new Modification() { ID = ++newID, JobOrderID = JobOrderData.ID, Date = DateTime.Now };
            modifications.Insert(0, newModification);
            ModificationsList.SelectedItem = newModification;

            ItemsList.ContextMenu = (ContextMenu)this.Resources["ItemsListContextMenu"];
            ItemsList.RowStyle = (Style)this.Resources["NewItems"];
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ModificationsList.SelectedItem is Modification modificationData)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    foreach (MItem item in items.Where(i => i.ModificationID == modificationData.ID))
                    {
                        if (item.ItemSort == -1)
                        {
                            query = $"Select Max(ItemSort) From [JobOrder].[PanelsItems] Where PanelID ={item.PanelID} And ItemTable = '{item.ItemTable}'";
                            item.ItemSort = (int)connection.ExecuteScalar(query) + 1;
                        }
                        query = $"Insert Into [JobOrder].[PanelsItems] " +
                                $"(PanelID, Category, Code,  Description, Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort, ModificationID, Source, Date) " +
                                $"Values " +
                                $"(@PanelID, @Category, @Code,  @Description, @Unit, @ItemQty, @Brand, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort, @ModificationID, @Source, @Date) " +
                                $"Select @@IDENTITY";

                        item.ItemID = (int)(decimal)connection.ExecuteScalar(query, item);
                    }
                }
                ItemsList.ContextMenu = null;
                ItemsList.RowStyle = (Style)this.Resources["Items"];
                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedItem is Modification modificationData)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                List<MItem> list = new List<MItem>();
                foreach (MItem item in items.Where(i => i.ModificationID == modificationData.ID))
                {
                    MItem jobOrderItem = jobOrderItems.FirstOrDefault(i => i.PartNumber == item.PartNumber);
                    jobOrderItem.ItemQty += item.ItemQty * -1;
                    list.Add(item);
                }

                foreach (MItem item in list)
                {
                    items.Remove(item);
                }


                ItemsList.ContextMenu = null;
                ItemsList.RowStyle = (Style)this.Resources["Items"];
                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedIndex > 0)
                ModificationsList.SelectedIndex -= 1;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedIndex < ModificationsList.Items.Count - 1)
                ModificationsList.SelectedIndex += 1;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is MPanel panel)
            {
                Items_Standard_Window window = new Items_Standard_Window()
                {
                    PanelID = panel.PanelID,
                    ItemData = null,
                    ActionData = Actions.New,
                    ModificationData = (ModificationsList.SelectedItem as Modification),
                    ItemsData = items,
                    ReferencesData = this.referencesData,
                    JobOrderItemsData = this.jobOrderItems,
                };
                window.ShowDialog();
            }
        }
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if(PanelsList.SelectedItem is MPanel panel)
            {
                Items_Standard_Window window = new Items_Standard_Window()
                {
                    PanelID = panel.PanelID,
                    ItemData = null,
                    ActionData = Actions.Remove,
                    ModificationData = (ModificationsList.SelectedItem as Modification),
                    ItemsData = items,
                    ReferencesData = this.referencesData,
                    JobOrderItemsData = this.jobOrderItems,
                };
                window.ShowDialog();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is MItem item)
            {
                if (ModificationsList.SelectedItem is Modification modification)
                {
                    if (item.ModificationID == modification.ID)
                    {
                        if(item.ItemType == ItemTypes.Standard.ToString())
                        {
                            Items_Standard_Window window = new Items_Standard_Window()
                            {
                                PanelID = (PanelsList.SelectedItem as MPanel).PanelID,
                                ItemData = item,
                                ActionData = Actions.Edit,
                                ModificationData = (ModificationsList.SelectedItem as Modification),
                                ItemsData = items,
                                ReferencesData = this.referencesData,
                                JobOrderItemsData = this.jobOrderItems,
                            };
                            window.ShowDialog();
                        }
                        else
                        {
                            if(item.Source == "Additional")
                            {
                                Items_New_Window window = new Items_New_Window()
                                {
                                    PanelID = (PanelsList.SelectedItem as MPanel).PanelID,
                                    ItemData = item,
                                    ActionData = Actions.Edit,
                                    ModificationData = (ModificationsList.SelectedItem as Modification),
                                    ItemsData = items,
                                    JobOrderItemsData = this.jobOrderItems,
                                };
                                window.ShowDialog();
                            }
                            else
                            {
                                Items_Standard_Window window = new Items_Standard_Window()
                                {
                                    PanelID = (PanelsList.SelectedItem as MPanel).PanelID,
                                    ItemData = item,
                                    ActionData = Actions.Edit,
                                    ModificationData = (ModificationsList.SelectedItem as Modification),
                                    ItemsData = items,
                                    ReferencesData = this.referencesData,
                                    JobOrderItemsData = this.jobOrderItems,
                                };
                                window.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        CMessageBox.Show("Posted Item!", "Can't edit posted items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    }
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is MItem item)
            {
                if(ModificationsList.SelectedItem is Modification modification)
                {
                    if (item.ModificationID == modification.ID)
                    {
                        MItem jobOrderItem = jobOrderItems.FirstOrDefault(i => i.PartNumber == item.PartNumber);
                        jobOrderItem.ItemQty += item.ItemQty * -1;
                        items.Remove(item);
                    }
                    else
                        CMessageBox.Show("Posted Item!", "Can't delete posted items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
    }
}
