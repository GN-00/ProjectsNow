using Dapper;
using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.DataInput;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class QuotationPanelsWindow : Window
    {
        public ObservableCollection<QPanel> panelsData;

        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }
        public ObservableCollection<Quotation> QuotationsData { get; set; }

        public QuotationPanelsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                panelsData = QPanelController.QuotationPanels(connection, QuotationData.QuotationID);
            }

            viewData = new CollectionViewSource() { Source = panelsData };
            viewData.Filter += DataFilter;

            PanelsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            DataContext = new { UserData, QuotationData };
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
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.QuotationID = null;
                UserController.UpdateQuotationID(connection, UserData);
            }
            this.Close();
        }

        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            var quotationWindow = new QuotationWindow()
            {
                UserData = this.UserData,
                QuotationData = QuotationData,
            };
            quotationWindow.ShowDialog();
        }
        private void TC_Click(object sender, RoutedEventArgs e)
        {
            var termsAndConditionsWindow = new TermsAndConditionsWindow()
            {
                UserData = this.UserData,
                QuotationData = QuotationData,
            };
            termsAndConditionsWindow.ShowDialog();
        }
        private void Prices_Click(object sender, RoutedEventArgs e)
        {
            var quotationPriceWindow = new QuotationPriceWindow()
            {
                UserData = this.UserData,
                QuotationData = QuotationData,
                ResetUserQuotationID = false,
            };
            quotationPriceWindow.ShowDialog();
        }
        private void Printer_Click(object sender, RoutedEventArgs e)
        {
            PrintQuotationWindow printQuotationWindow = new PrintQuotationWindow()
            {
                QuotationData = QuotationData,
                UserData = this.UserData,
            };
            printQuotationWindow.ShowDialog();
        }

        private void Material_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                QuotationPanelItemsWindow panelItemsWindow = new QuotationPanelItemsWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                    QuotationData = this.QuotationData,
                    QuotationPanelsWindowData = this,
                };
                panelItemsWindow.ShowDialog();
            }
        }

        private void BillPriceOptions_Click(object sender, RoutedEventArgs e)
        {
            QuotationBillPriceOptionsWindow quotationBillPriceOptionsWindow = new QuotationBillPriceOptionsWindow()
            {
                UserData = this.UserData,
                QuotationData = this.QuotationData,
            };
            quotationBillPriceOptionsWindow.ShowDialog();
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(QPanel)).GetProperty("PanelSN"),
            (typeof(QPanel)).GetProperty("PanelName"),
            (typeof(QPanel)).GetProperty("PanelQty"),
            (typeof(QPanel)).GetProperty("EnclosureType"),
            (typeof(QPanel)).GetProperty("EnclosureHeight"),
            (typeof(QPanel)).GetProperty("EnclosureWidth"),
            (typeof(QPanel)).GetProperty("EnclosureDepth"),
            (typeof(QPanel)).GetProperty("EnclosureIP"),
            (typeof(QPanel)).GetProperty("PanelProfit"),
            (typeof(QPanel)).GetProperty("PanelCost"),
            (typeof(QPanel)).GetProperty("PanelPrice"),
            (typeof(QPanel)).GetProperty("PanelsCost"),
            (typeof(QPanel)).GetProperty("PanelsPrice"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is QPanel record)
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

        private void PanelInfo_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                QuotationPanelWindow quotationPanelWindow = new QuotationPanelWindow()
                {
                   PanelData = panelData,
                };
                quotationPanelWindow.ShowDialog();
            }
        }

        private void QuotationItems_ClicK(object sender, RoutedEventArgs e)
        {
            List<QItem> items;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"SELECT Quotation.QuotationsPanels.QuotationID, Quotation.QuotationsPanelsItems.Category, Quotation.QuotationsPanelsItems.Code, Quotation.QuotationsPanelsItems.Description, " +
                               $"SUM(Quotation.QuotationsPanelsItems.ItemQty) AS ItemQty " +
                               $"FROM Quotation.QuotationsPanelsItems INNER JOIN " +
                               $"Quotation.QuotationsPanels ON Quotation.QuotationsPanelsItems.PanelID = Quotation.QuotationsPanels.PanelID " +
                               $"WHERE(Quotation.QuotationsPanelsItems.ItemCost <> 0) " +
                               $"GROUP BY Quotation.QuotationsPanels.QuotationID, Quotation.QuotationsPanelsItems.Description, Quotation.QuotationsPanelsItems.Code, Quotation.QuotationsPanelsItems.Category " +
                               $"HAVING (Quotation.QuotationsPanels.QuotationID = {QuotationData.QuotationID}) " +
                               $"ORDER BY Quotation.QuotationsPanelsItems.Code";

                items = connection.Query<QItem>(query).ToList();
            }

            for (int i = 1; i <= items.Count; i++)
                items[i - 1].ItemSort = i;

            double pagesNumber = (items.Count) / 45d;
            if (pagesNumber - Convert.ToInt32(pagesNumber) != 0)
                pagesNumber = Convert.ToInt32(pagesNumber) + 1;

            List<FrameworkElement> elements = new List<FrameworkElement>();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.QuotationsItems quotationsItems = new Printing.QuotationsItems() { QuotationData = QuotationData, Items = items.Where(p => p.ItemSort > ((i - 1) * 45) && p.ItemSort <= ((i) * 45)).ToList(), Page = i, Pages = Convert.ToInt32(pagesNumber) };
                    elements.Add(quotationsItems);
                }

                Printing.Print.PrintPreview(elements);
            }
            else
            {
                CMessageBox.Show("Items", "There is no items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }
    }
}
