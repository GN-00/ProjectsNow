using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.InquiryWindows;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class QuotationsInformationWindow : Window
    {
        bool isLoading;
        Statuses status;
        ObservableCollection<QuotationsYear> YearsData;
        ObservableCollection<Quotation> QuotationsData;

        public User UserData { get; set; }

        public QuotationsInformationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoading = true;
            status = Statuses.All;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                QuotationsData = QuotationController.GetQuotations(connection, DateTime.Today.Year, status);
                YearsList.ItemsSource = YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = QuotationsData };
            viewData.Filter += DataFilter;

            QuotationsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            YearsList.SelectedItem = YearsData.Where(item => item.Year == DateTime.Today.Year).FirstOrDefault();
            YearValue.Text = DateTime.Today.Year.ToString();

            if (QuotationsData.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (UserData.QuotationsManage != true)
            {
                Manage.IsSelected = false;
                Manage.Visibility = Visibility.Collapsed;
                Tool.IsSelected = true;
            }
            StatusName.Foreground = YearValue.Foreground = Brushes.Black;
            isLoading = false;
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
            var selectedIndex = QuotationsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Quotations: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Quotation: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void QuotationsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = QuotationsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Quotations: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Quotation: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void Edit_ClicK(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotation)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserInquiryID(connection, quotation.InquiryID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.InquiryID = quotation.InquiryID;
                        UserController.UpdateInquiryID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var inquiry = new Inquiry();
                    inquiry.QuotationToInquiry(quotation);

                    var inquiryWindow = new InquiryWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = quotation,
                        WindowMode = Actions.Edit,
                        InquiryData = inquiry,
                    };
                    inquiryWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This inquiry underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotation)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserInquiryID(connection, quotation.InquiryID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.InquiryID = quotation.InquiryID;
                        UserController.UpdateInquiryID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var assignWindow = new AssignWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = quotation,
                    };
                    assignWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This inquiry underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserQuotationID(connection, QuotationData.QuotationID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.QuotationID = QuotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                   
                    var quotationWindow = new QuotationWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = QuotationData,
                    };
                    quotationWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void TC_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserQuotationID(connection, QuotationData.QuotationID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.QuotationID = QuotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var termsAndConditionsWindow = new TermsAndConditionsWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = QuotationData,
                    };
                    termsAndConditionsWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void QuotationPanels_ClicK(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserQuotationID(connection, QuotationData.QuotationID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.QuotationID = QuotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var quotationPanelsWindow = new QuotationPanelsWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = QuotationData,
                    };
                    quotationPanelsWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Prices_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserQuotationID(connection, QuotationData.QuotationID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.QuotationID = QuotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var quotationPriceWindow = new QuotationPriceWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = QuotationData,
                        ResetUserQuotationID = true,
                    };
                    quotationPriceWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Printer_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                PrintQuotationWindow printQuotationWindow = new PrintQuotationWindow()
                {
                    QuotationData = quotationData,
                    UserData = this.UserData,
                };
                printQuotationWindow.ShowDialog();
            }
        }
        private void BillPriceOptions_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserQuotationID(connection, quotationData.QuotationID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.QuotationID = quotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    QuotationBillPriceOptionsWindow quotationBillPriceOptionsWindow = new QuotationBillPriceOptionsWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = quotationData,
                    };
                    quotationBillPriceOptionsWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void AllQuotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.All;
            StatusName.Text = $"{status} Quotations";
            StatusName.Foreground = YearValue.Foreground = Brushes.Black;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Today.Year, status);
                YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            isLoading = false;
        }
        private void RunningQuotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Running;
            StatusName.Text = $"{status} Quotations";
            StatusName.Foreground = YearValue.Foreground = Brushes.Green;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Today.Year, status);
                YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            isLoading = false;
        }
        private void SubmittedQuotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Submitted;
            StatusName.Text = $"{status} Quotations";
            StatusName.Foreground = YearValue.Foreground = Brushes.Blue;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Today.Year, status);
                YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            isLoading = false;
        }
        private void HoldedQuotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Hold;
            StatusName.Text = $"{status} Quotations";
            StatusName.Foreground = YearValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFC800"));
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Today.Year, status);
                YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            isLoading = false;
        }
        private void CanceledQuotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Cancelled;
            StatusName.Text = $"{status} Quotations";
            StatusName.Foreground = YearValue.Foreground = Brushes.Red;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Today.Year, status);
                YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            isLoading = false;
        }
        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (YearsList.SelectedItem is QuotationsYear yearData)
            {
                DeleteFilter_Click(sender, e);
                YearValue.Text = yearData.Year.ToString();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, yearData.Year, status);
                }
                QuotationsList.ItemsSource = viewData.View;
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(Quotation)).GetProperty("QuotationCode"),
            (typeof(Quotation)).GetProperty("CustomerName"),
            (typeof(Quotation)).GetProperty("ProjectName"),
            (typeof(Quotation)).GetProperty("EstimationName"),
            (typeof(Quotation)).GetProperty("RegisterDate"),
            (typeof(Quotation)).GetProperty("DuoDate"),
            (typeof(Quotation)).GetProperty("Priority"),
            (typeof(Quotation)).GetProperty("QuotationStatus"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Quotation record)
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

        private void DiscountSheets_ClicK(object sender, RoutedEventArgs e)
        {
            DiscountSheetsWindow discountSheetsWindow = new DiscountSheetsWindow();
            discountSheetsWindow.ShowDialog();
        }

        private void QuotationItems_ClicK(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                //int numberRow = 45;
                List<QItem> items;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"SELECT Quotation.QuotationsPanels.QuotationID, Quotation.QuotationsPanelsItems.Category, Quotation.QuotationsPanelsItems.Code, Quotation.QuotationsPanelsItems.Description, " +
                                   $"SUM(Quotation.QuotationsPanelsItems.ItemQty) AS ItemQty " +
                                   $"FROM Quotation.QuotationsPanelsItems INNER JOIN " +
                                   $"Quotation.QuotationsPanels ON Quotation.QuotationsPanelsItems.PanelID = Quotation.QuotationsPanels.PanelID " +
                                   $"WHERE(Quotation.QuotationsPanelsItems.ItemCost <> 0) " +
                                   $"GROUP BY Quotation.QuotationsPanels.QuotationID, Quotation.QuotationsPanelsItems.Description, Quotation.QuotationsPanelsItems.Code, Quotation.QuotationsPanelsItems.Category " +
                                   $"HAVING (Quotation.QuotationsPanels.QuotationID = {quotationData.QuotationID}) " +
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
                        Printing.QuotationsItems quotationsItems = new Printing.QuotationsItems() { QuotationData = quotationData, Items = items.Where(p => p.ItemSort > ((i - 1) * 45) && p.ItemSort <= ((i) * 45)).ToList(), Page = i, Pages = Convert.ToInt32(pagesNumber) };
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
}
