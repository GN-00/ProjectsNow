using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Reflection;
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

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationBillPriceOptionsWindow : Window
    {
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }

        ObservableCollection<QuotationOption> quotationOptions;
        ObservableCollection<QuotationOptionPanel> quotationOptionPanels;
        public QuotationBillPriceOptionsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string IDs = "";
                quotationOptions = QuotationOptionController.GetOptions(connection, QuotationData.QuotationID);

                if (quotationOptions.Count != 0)
                {
                    foreach (QuotationOption option in quotationOptions)
                    {
                        IDs += $"{option.ID}, ";
                    }
                    IDs = IDs.Substring(0, IDs.Length - 2);
                    quotationOptionPanels = QuotationOptionPanelController.GetPanels(connection, IDs);
                }
                else
                {
                    quotationOptionPanels = new ObservableCollection<QuotationOptionPanel>();
                }
            }

            viewDataOptions = new CollectionViewSource() { Source = quotationOptions };
            viewDataPanels = new CollectionViewSource() { Source = quotationOptionPanels };

            OptionsList.ItemsSource = viewDataOptions.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataPanels.Filter += DataFilter;

            viewDataOptions.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Options_CollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Panels_CollectionChanged);

            if (viewDataOptions.View.Cast<object>().Count() == 0)
                Options_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewDataPanels.View.Cast<object>().Count() == 0)
                Panels_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            DataContext = new { QuotationData };
        }
        private void Options_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = OptionsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationOptions.Text = $"Options: {viewDataOptions.View.Cast<object>().Count()}";
            else
                NavigationOptions.Text = $"Option: {selectedIndex + 1} / {viewDataOptions.View.Cast<object>().Count()}";
        }
        private void Panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        #region Filters

        CollectionViewSource viewDataOptions;
        CollectionViewSource viewDataPanels;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(QuotationOptionPanel)).GetProperty("OptionID"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is QuotationOptionPanel record)
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

                        if (OptionsList.SelectedItem is QuotationOption optionData)
                            if (value != optionData.ID.ToString())
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
        private void OptionsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = OptionsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationOptions.Text = $"Options: {viewDataOptions.View.Cast<object>().Count()}";
            else
                NavigationOptions.Text = $"Option: {selectedIndex + 1} / {viewDataOptions.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
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

        private void Printer_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsList.SelectedItem is QuotationOption optionData)
            {
                var panels = quotationOptionPanels.Where(p => p.OptionID == optionData.ID);
                if (panels.Count() != 0)
                {
                    string PanelsIDs = "";
                    foreach (QuotationOptionPanel panel in panels)
                    {
                        PanelsIDs += panel.PanelID + ", ";
                    }
                    PanelsIDs = PanelsIDs.Substring(0, PanelsIDs.Length - 2);

                    Quotation newQuotation = new Quotation();
                    newQuotation.Update(QuotationData);
                    newQuotation.QuotationCode = newQuotation.QuotationCode.Insert(6, optionData.Code);

                    PrintQuotationWindow printQuotationWindow = new PrintQuotationWindow()
                    {
                        QuotationOptionData = optionData,
                        QuotationData = newQuotation,
                        UserData = this.UserData,
                        PanelIDs = PanelsIDs,
                    };
                    printQuotationWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show("Panels!!", "This option do not have panels!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void NewOption_Click(object sender, RoutedEventArgs e)
        {
            QuotationOptionWindow quotationOptionWindow = new QuotationOptionWindow()
            {
                ActionData = Actions.New,
                QuotationOptionsData = this.quotationOptions,
                QuotationID = this.QuotationData.QuotationID,
            };
            quotationOptionWindow.ShowDialog();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsList.SelectedItem is QuotationOption quotationOptionData)
            {
                QuotationOptionWindow quotationOptionWindow = new QuotationOptionWindow()
                {
                    ActionData = Actions.Edit,
                    QuotationOptionData = quotationOptionData,
                    QuotationOptionsData = this.quotationOptions,
                    QuotationID = this.QuotationData.QuotationID,
                };
                quotationOptionWindow.ShowDialog();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsList.SelectedItem is QuotationOption optionData)
            {
                if (quotationOptionPanels == null)
                    quotationOptionPanels = new ObservableCollection<QuotationOptionPanel>();

                QuotationOptionPanelsWindow quotationOptionPanelsWindow = new QuotationOptionPanelsWindow()
                {
                    QuotationID = this.QuotationData.QuotationID,
                    OptionID = optionData.ID,
                    OptionPanels = this.quotationOptionPanels,
                };
                quotationOptionPanelsWindow.ShowDialog();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsList.SelectedItem is QuotationOption quotationOptionData)
            {
                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Delete From [Quotation].[QuotationsOptions] Where ID = {quotationOptionData.ID}; ";
                    query += $"Delete From [Quotation].[QuotationsOptionsPanels] Where OptionID = {quotationOptionData.ID} ";
                    connection.Execute(query);
                }

                quotationOptions.Remove(quotationOptionData);
                if (quotationOptions.Count == 0)
                    quotationOptionPanels.Clear();
            }
        }

        private void RemovePanel_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QuotationOptionPanel panelData)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = $"Delete From [Quotation].[QuotationsOptionsPanels] Where PanelID = {panelData.PanelID} ";
                    connection.Execute(query);
                }

                quotationOptionPanels.Remove(panelData);
            }
        }
    }
}
