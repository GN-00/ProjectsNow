using Dapper;
using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class Panels_Design_Window : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        readonly string filter1 = Statuses.New.ToString();
        public Panels_Design_Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewData1 = new CollectionViewSource() { Source = PanelsData };
            viewData2 = new CollectionViewSource() { Source = PanelsData };

            viewData1.Filter += Data1Filter;
            viewData2.Filter += Data2Filter;

            List1.ItemsSource = viewData1.View;
            List2.ItemsSource = viewData2.View;

            viewData1.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            viewData2.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData1.View.Cast<object>().Count() == 0 || viewData2.View.Cast<object>().Count() == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

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

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (List1.SelectedItem is JPanel panelData)
            {
                panelData.Status = Statuses.Designing.ToString();
                panelData.DateOfDesign = DateTime.Today;

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"Update [JobOrder].[Panels] Set " +
                                   $"Status = @Status , " +
                                   $"DateOfDesign = @DateOfDesign " +
                                   $"Where PanelID = @PanelID";

                    connection.Execute(query, panelData);
                }
                viewData1.View.Refresh();
                viewData2.View.Refresh();
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (List2.SelectedItem is JPanel panelData)
            {
                if (panelData.Status == Statuses.Designing.ToString())
                {
                    panelData.Status = Statuses.New.ToString();
                    panelData.DateOfDesign = null;

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Update [JobOrder].[Panels] Set " +
                                       $"Status = @Status , " +
                                       $"DateOfDesign = @DateOfDesign " +
                                       $"Where PanelID = @PanelID";

                        connection.Execute(query, panelData);
                    }
                    viewData1.View.Refresh();
                    viewData2.View.Refresh();
                }
                else
                {
                    if (panelData.Status == Statuses.Waiting_Approval.ToString())
                        CMessageBox.Show("Panels", "The panel is awaiting approval!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    else if (panelData.Status == Statuses.Production.ToString())
                        CMessageBox.Show("Panels", "The panel under fabrication!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    else if (panelData.Status == Statuses.Closed.ToString())
                        CMessageBox.Show("Panels", "The panel closed!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    else if (panelData.Status == Statuses.Hold.ToString())
                        CMessageBox.Show("Panels", "The panel is hold!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    else if (panelData.Status == Statuses.Cancelled.ToString())
                        CMessageBox.Show("Panels", "The panel is Canceled!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            else
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";

            selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            else
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
        }

        private void List1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            else
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";
        }

        private void List2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            else
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
        }

        #region Filters

        CollectionViewSource viewData1;
        CollectionViewSource viewData2;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(JPanel)).GetProperty("Status"),
        };
        private void Data1Filter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";

                        if (value != filter1)
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

        private void Data2Filter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel panel)
                {
                    if (panel.DateOfDesign == null)
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
