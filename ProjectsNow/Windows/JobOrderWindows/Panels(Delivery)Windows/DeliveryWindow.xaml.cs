using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Events;
using System.Windows.Data;
using ProjectsNow.Printing;
using System.Windows.Input;
using ProjectsNow.Database;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Delivery_Windows
{
    public partial class DeliveryWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        ObservableCollection<Delivery> deliveries;
        ObservableCollection<JPanel> tempPanelsData;
        ObservableCollection<TPanel> panelsTransaction;
        public DeliveryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Panels(Delivered)] Where JobOrderID  = {JobOrderData.ID} ";
                panelsTransaction = new ObservableCollection<TPanel>(connection.Query<TPanel>(query));
            }

            deliveries = new ObservableCollection<Delivery>(panelsTransaction.GroupBy(i => i.Reference, ii => ii.Date).Select(m => new Delivery { Number = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.Number));

            viewDataDeliveries = new CollectionViewSource() { Source = deliveries };
            viewDataPanels = new CollectionViewSource() { Source = panelsTransaction };

            viewDataPanels.Filter += DataFilter;
            DeliveriesList.ItemsSource = viewDataDeliveries.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataDeliveries.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DeliveriesCollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PanelsCollectionChanged);

            if (viewDataDeliveries.View.Cast<object>().Count() == 0)
                DeliveriesCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewDataPanels.View.Cast<object>().Count() == 0)
                PanelsCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            tempPanelsData = new ObservableCollection<JPanel>();
            foreach (JPanel panel in PanelsData)
            {
                JPanel newPanel = new JPanel();
                newPanel.Update(panel);
                tempPanelsData.Add(newPanel);
            }

            DataContext = new { JobOrderData, UserData };
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
        private void DeliveriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = DeliveriesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationDeliveries.Text = $"Deliveries: {viewDataDeliveries.View.Cast<object>().Count()}";
            else
                NavigationDeliveries.Text = $"Delivery: {selectedIndex + 1} / {viewDataDeliveries.View.Cast<object>().Count()}";
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataPanels.View.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            }
        }
        private void DeliveriesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = DeliveriesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationDeliveries.Text = $"Deliveries: {viewDataDeliveries.View.Cast<object>().Count()}";
            else
                NavigationDeliveries.Text = $"Delivery: {selectedIndex + 1} / {viewDataDeliveries.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
        }

        #region Filters
        CollectionViewSource viewDataDeliveries;
        CollectionViewSource viewDataPanels;
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is TPanel panel)
                {
                    if (DeliveriesList.SelectedItem is Delivery delivey)
                        if (delivey.Number != panel.Reference)
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

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Visible;
            ControlLock2.Visibility = Visibility.Collapsed;

            int deliveryNumber;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select DeliveryNumber From [JobOrder].[DeliveryNumber] Where Year = {DateTime.Now.Year}";
                deliveryNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            }

            var newDelivery = new Delivery() { Number = $"{deliveryNumber:000}{DateTime.Now.Month}{DateTime.Now.Year.ToString().Substring(2,2)}", Date = DateTime.Now };
            deliveries.Insert(0, newDelivery);
            DeliveriesList.SelectedItem = newDelivery;

            LoadingControl.Visibility = Visibility.Visible;

            PanelsList.ContextMenu = (ContextMenu)this.Resources["PanelsListContextMenu"];
            PanelsList.RowStyle = (Style)this.Resources["NewPanels"];
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    foreach (TPanel panel in panelsTransaction.Where(i => i.Reference == delivery.Number))
                    {
                        query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                                $"(PanelID, Reference, Qty, Date, Action, JobOrderID) " +
                                $"Values " +
                                $"(@PanelID, @Reference, @Qty, @Date, 'Delivered', @JobOrderID) Select @@IDENTITY";
                        panel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, panel);

                        JPanel panelData = PanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                        panelData.DeliveredQty += panel.Qty;

                        if (panelData.PanelQty == panelData.DeliveredQty)
                        {
                            panelData.Status = Statuses.Delivered.ToString();
                            connection.Execute($"Update [JobOrder].[Panels] Set Status ='{Statuses.Delivered}' Where PanelID = {panelData.PanelID}");
                        }
                    }
                }

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();

                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                List<TPanel> list = new List<TPanel>();
                foreach (TPanel panel in panelsTransaction.Where(i => i.Reference == delivery.Number))
                {
                    JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.DeliveredQty += panel.Qty * -1;
                    list.Add(panel);
                }

                foreach (TPanel panel in list)
                {
                    panelsTransaction.Remove(panel);
                }

                deliveries.Remove(delivery);

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery deliveryData)
            {
                string query;
                List<DPanel> panels;
                DeliveryInfromation deliveryInfromation;
                List<int> deliveriesNumbers = new List<int>();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[DeliveriesInformations] Where DeliveryNumber = {deliveryData.Number}";
                    deliveryInfromation = connection.QueryFirstOrDefault<DeliveryInfromation>(query);

                    query = $"Select * From [JobOrder].[Panels(DeliveryDetails)] Where DeliveryNumber = {deliveryData.Number}";
                    panels = connection.Query<DPanel>(query).ToList();
                }

                foreach (Delivery delivery in deliveries)
                    deliveriesNumbers.Add(int.Parse(delivery.Number.Substring(0, 3)));

                deliveriesNumbers.Sort();

                deliveryInfromation.ShipmentNo = deliveriesNumbers.IndexOf(int.Parse(deliveryData.Number.Substring(0,3))) + 1;

                foreach (DPanel panel in panels)
                    panel.PreviousQty = panelsTransaction.Where(p => p.PanelID == panel.PanelID && int.Parse(p.Reference) < int.Parse(panel.DeliveryNumber)).Sum(p => p.Qty);

                for (int i = 1; i <= panels.Count; i++)
                    panels[i - 1].PanelSN = i;

                double pagesNumber = (panels.Count) / 10d;
                if (pagesNumber - Convert.ToInt32(pagesNumber) != 0)
                    pagesNumber = Convert.ToInt32(pagesNumber) + 1;


                if (pagesNumber != 0)
                {
                    List<FrameworkElement> elements = new List<FrameworkElement>();
                    for (int i = 1; i <= pagesNumber; i++)
                    {
                        DeliveryForm deliveryForm = new DeliveryForm()
                        {
                            Page = i,
                            Pages = Convert.ToInt32(pagesNumber),
                            DeliveryInfromation = deliveryInfromation,
                            PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 10) && p.PanelSN <= ((i) * 10)).ToList()
                        };

                        elements.Add(deliveryForm);
                    }

                    Print.PrintPreview(elements, $"Delivery-{deliveryData.Number}");
                }
                else
                {
                    CMessageBox.Show("Items", "There is no panels!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void AddPanels_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                var window = new PanelsWindow()
                {
                    DeliveryData = delivery,
                    PanelsData = tempPanelsData,
                    PanelsTransaction = panelsTransaction,
                };
                window.ShowDialog();
            }
        }
        private void DeletePanels_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is TPanel panel)
            {
                JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                panelData.DeliveredQty += panel.Qty * -1;
                panelsTransaction.Remove(panel);
            }
        }
    }
}
