using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Approval_Windows
{
    public partial class PanelsWindow : Window
    {
        public ApprovalRequest RequestData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }
        public ObservableCollection<TPanel> PanelsTransaction { get; set; }

        CollectionViewSource viewData;
        public PanelsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewData = new CollectionViewSource() { Source = PanelsData };
            viewData.Filter += DataFilter;

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
            if (PanelsList.SelectedItem is JPanel panelData)
            {
                panelData.Status = Statuses.Waiting_Approval.ToString();
                TPanel newPanel = new TPanel()
                {
                    JobOrderID = panelData.JobOrderID,
                    PanelID = panelData.PanelID,
                    PanelSN = panelData.PanelSN,
                    PanelName = panelData.PanelName,
                    EnclosureType = panelData.EnclosureType,
                    Qty = panelData.PanelQty,
                    Reference = RequestData.Number,
                    Date = RequestData.Date,
                };
                PanelsTransaction.Add(newPanel);

                this.Close();
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = false;
                if (e.Item is JPanel panel)
                {
                    if (panel.Status == Statuses.Designing.ToString())
                    {
                        e.Accepted = true;
                        return;
                    }
                }
            }
            catch
            {
                e.Accepted = false;
            }
        }
        #endregion
    }
}
