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

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Approval_Windows
{
    public partial class RequestsWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        ObservableCollection<ApprovalRequest> requests;
        ObservableCollection<JPanel> tempPanelsData;
        ObservableCollection<TPanel> panelsTransaction;
        public RequestsWindow()
        {
            InitializeComponent();
            JobOrderData = new JobOrder() { ID = 0 };
            PanelsData = new ObservableCollection<JPanel>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Panels(Waiting_Approval)] Where JobOrderID  = {JobOrderData.ID} ";
                panelsTransaction = new ObservableCollection<TPanel>(connection.Query<TPanel>(query));
            }

            requests = new ObservableCollection<ApprovalRequest>(panelsTransaction.GroupBy(i => i.Reference, ii => ii.Date).Select(m => new ApprovalRequest { Number = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.Number));

            viewDataRequests = new CollectionViewSource() { Source = requests };
            viewDataPanels = new CollectionViewSource() { Source = panelsTransaction };

            viewDataPanels.Filter += DataFilter;
            RequestsList.ItemsSource = viewDataRequests.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataRequests.View.CollectionChanged += new NotifyCollectionChangedEventHandler(RequestsCollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PanelsCollectionChanged);

            if (viewDataRequests.View.Cast<object>().Count() == 0)
                RequestsCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

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
        private void RequestsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = RequestsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationRequests.Text = $"Requests: {viewDataRequests.View.Cast<object>().Count()}";
            else
                NavigationRequests.Text = $"Request: {selectedIndex + 1} / {viewDataRequests.View.Cast<object>().Count()}";
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
        private void RequestsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = RequestsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationRequests.Text = $"Requests: {viewDataRequests.View.Cast<object>().Count()}";
            else
                NavigationRequests.Text = $"Request: {selectedIndex + 1} / {viewDataRequests.View.Cast<object>().Count()}";
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
        CollectionViewSource viewDataRequests;
        CollectionViewSource viewDataPanels;
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is TPanel panel)
                {
                    if (RequestsList.SelectedItem is ApprovalRequest request)
                        if (request.Number != panel.Reference)
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
                string query = $"Select RequestNumber From [JobOrder].[RequestNumber] Where JobOrderID = {JobOrderData.ID}";
                deliveryNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            }

            var newRequest = new ApprovalRequest() { Number = $"{deliveryNumber:000}", Date = DateTime.Now };
            requests.Insert(0, newRequest);
            RequestsList.SelectedItem = newRequest;

            LoadingControl.Visibility = Visibility.Visible;

            PanelsList.ContextMenu = (ContextMenu)this.Resources["PanelsListContextMenu"];
            PanelsList.RowStyle = (Style)this.Resources["NewPanels"];
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is ApprovalRequest delivery)
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
                                $"(@PanelID, @Reference, @Qty, @Date, 'Waiting_Approval', @JobOrderID) Select @@IDENTITY";
                        panel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, panel);

                        JPanel panelData = PanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                        panelData.DeliveredQty += panel.Qty;

                        panelData.Status = Statuses.Waiting_Approval.ToString();
                        connection.Execute($"Update [JobOrder].[Panels] Set Status ='{Statuses.Waiting_Approval}' Where PanelID = {panelData.PanelID}");
                    }
                }

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is ApprovalRequest delivery)
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

                requests.Remove(delivery);

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is ApprovalRequest requestData)
            {
                string query;
                List<APanel> panels;
                List<string> POs;
                ApprovalRequestInformation requestInfromation;
                List<int> requestsNumbers = new List<int>();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[ApprovalRequestsInformations] Where JobOrderID = {JobOrderData.ID} And RequestNumber = {requestData.Number}";
                    requestInfromation = connection.QueryFirstOrDefault<ApprovalRequestInformation>(query);

                    query = $"Select * From [JobOrder].[Panels(ApprovalRequestDetails)] Where JobOrderID = {JobOrderData.ID} And RequestNumber = {requestData.Number}";
                    panels = connection.Query<APanel>(query).ToList();
                }

                POs = panels.GroupBy(p => p.PurchaseOrdersNumber).Select(p => p.Key).ToList();

                for (int i = 1; i <= panels.Count; i++)
                    panels[i - 1].PanelSN = i;

                foreach (string po in POs)
                    requestInfromation.POs += $"{po}, ";

                requestInfromation.POs = requestInfromation.POs.Substring(0, requestInfromation.POs.Length - 2);
                requestInfromation.DrawingsNo = panels.Count;

                double pagesNumber = (panels.Count) / 10d;
                if (pagesNumber - Convert.ToInt32(pagesNumber) != 0)
                    pagesNumber = Convert.ToInt32(pagesNumber) + 1;


                if (pagesNumber != 0)
                {
                    List<FrameworkElement> elements = new List<FrameworkElement>();
                    for (int i = 1; i <= pagesNumber; i++)
                    {
                        RequestforShopDrawingApprovalControl requestforShopDrawingApproval = new RequestforShopDrawingApprovalControl()
                        {
                            Page = i,
                            Pages = Convert.ToInt32(pagesNumber),
                            RequestInformation = requestInfromation,
                            PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 10) && p.PanelSN <= ((i) * 10)).ToList()
                        };

                        elements.Add(requestforShopDrawingApproval);
                    }

                    Print.PrintPreview(elements, $"Shop Drawing Approval Request {requestData.Number}");
                }
                else
                {
                    CMessageBox.Show("Items", "There is no panels!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void AddPanels_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is ApprovalRequest delivery)
            {
                //var tempPanels = new ObservableCollection<JPanel>();
                //foreach (JPanel panel in tempPanelsData)
                //{
                //    JPanel newPanel = new JPanel();
                //    newPanel.Update(panel);
                //    tempPanels.Add(newPanel);
                //}

                var window = new PanelsWindow()
                {
                    RequestData = delivery,
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
                panelData.Status = Statuses.Designing.ToString();
                panelsTransaction.Remove(panel);
            }
        }

        private void Revise_Click(object sender, RoutedEventArgs e)
        {
            if(PanelsList.SelectedItem is TPanel panelData)
            {
                JPanel checkPanelData = PanelsData.Single(p => p.PanelID == panelData.PanelID);
                JPanel checkPanelData1 = tempPanelsData.Single(p => p.PanelID == panelData.PanelID);
                if (checkPanelData.Status == Statuses.Waiting_Approval.ToString())
                {
                    checkPanelData.Status = checkPanelData1.Status = Statuses.Designing.ToString();
                    checkPanelData.Revision++;
                    checkPanelData1.Revision++;

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Update [JobOrder].[Panels] Set " +
                                       $"Revision = {checkPanelData.Revision}, " +
                                       $"Status = '{checkPanelData.Status}' " +
                                       $"Where PanelID = {checkPanelData.PanelID}";

                        connection.Execute(query);
                    }
                }
                else
                {
                    CMessageBox.Show("Revise", $"Can't revise this panel!!\nStatus: {checkPanelData.Status}", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }

            }
        }
    }
}
