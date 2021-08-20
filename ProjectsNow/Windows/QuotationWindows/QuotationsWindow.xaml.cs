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
using Excel = Microsoft.Office.Interop.Excel;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationsWindow : Window
    {
        bool isLoading;
        Statuses status;
        ObservableCollection<QuotationsYear> YearsData;
        ObservableCollection<Quotation> QuotationsData;

        public User UserData { get; set; }

        public QuotationsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoading = true;
            status = Statuses.Running;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (UserData.QuoteManager)
                {
                    QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsList.ItemsSource = YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, Statuses.Running);
                    YearsList.ItemsSource = YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = QuotationsData };
            viewData.Filter += DataFilter;

            QuotationsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            YearsList.SelectedItem = YearsData.Where(item => item.Year == DateTime.Now.Year).FirstOrDefault();
            YearValue.Text = DateTime.Now.Year.ToString();

            if (QuotationsData.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            StatusName.Foreground = YearValue.Foreground = Brushes.Green;
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
            {
                NavigationPanel.Text = $"Quotations: {viewData.View.Cast<object>().Count()}";
                AddToCancel.Visibility = AddToHolded.Visibility = AddToRunning.Visibility = Submit.Visibility = Revise.Visibility = Visibility.Collapsed;
            }
            else
            {
                NavigationPanel.Text = $"Quotation: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
                StatusChange();
            }
        }
        private void QuotationsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = QuotationsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPanel.Text = $"Quotations: {viewData.View.Cast<object>().Count()}";
                AddToCancel.Visibility = AddToHolded.Visibility = AddToRunning.Visibility = Submit.Visibility = Revise.Visibility = Visibility.Collapsed;
            }
            else
            {
                NavigationPanel.Text = $"Quotation: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
                StatusChange();
            }
        }

        public void StatusChange()
        {
            AddToRunning.Visibility = AddToHolded.Visibility = AddToCancel.Visibility = Submit.Visibility = Revise.Visibility = Visibility.Visible;
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                if (quotationData.QuotationStatus == Statuses.Running.ToString()) AddToRunning.Visibility = Revise.Visibility = Visibility.Collapsed;
                if (quotationData.QuotationStatus == Statuses.Hold.ToString()) AddToHolded.Visibility = Submit.Visibility = Revise.Visibility = Visibility.Collapsed;
                if (quotationData.QuotationStatus == Statuses.Cancelled.ToString()) AddToCancel.Visibility = Submit.Visibility = Revise.Visibility = Visibility.Collapsed;
                if (quotationData.QuotationStatus == Statuses.Submitted.ToString()) AddToCancel.Visibility = AddToHolded.Visibility = AddToRunning.Visibility = Submit.Visibility = Visibility.Collapsed;
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
                    if (QuotationData.QuotationStatus == Statuses.Running.ToString())
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
                        var quotationWindow = new QuotationsInformationWindows.QuotationWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = QuotationData,
                        };
                        quotationWindow.ShowDialog();
                    }
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
                    if (QuotationData.QuotationStatus == Statuses.Running.ToString())
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
                        var termsAndConditionsWindow = new QuotationsInformationWindows.TermsAndConditionsWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = QuotationData,
                        };
                        termsAndConditionsWindow.ShowDialog();
                    }

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
                    if (QuotationData.QuotationStatus == Statuses.Running.ToString())
                    {
                        var quotationPanelsWindow = new QuotationPanelsWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = QuotationData,
                            QuotationsData = QuotationsData,
                        };
                        quotationPanelsWindow.ShowDialog();
                    }
                    else
                    {
                        var quotationPanelsWindow = new QuotationsInformationWindows.QuotationPanelsWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = QuotationData,
                            QuotationsData = QuotationsData,
                        };
                        quotationPanelsWindow.ShowDialog();
                    }
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
                    if (QuotationData.QuotationStatus == Statuses.Running.ToString())
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
                        var quotationPriceWindow = new QuotationsInformationWindows.QuotationPriceWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = QuotationData,
                            ResetUserQuotationID = true,
                        };
                        quotationPriceWindow.ShowDialog();
                    }
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
                var result = CMessageBox.Show("Printing", "Print with watermark?", CMessageBoxButton.YesNo, CMessageBoxImage.Question);

                PrintQuotationWindow printQuotationWindow = new PrintQuotationWindow()
                {
                    QuotationData = quotationData,
                    UserData = this.UserData,
                };
                 if (result == MessageBoxResult.Yes) printQuotationWindow.BackgroundImage.IsChecked = true;
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
                    if (quotationData.QuotationStatus == Statuses.Running.ToString())
                    {
                        var quotationBillPriceOptionsWindow = new QuotationBillPriceOptionsWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = quotationData,
                        };
                        quotationBillPriceOptionsWindow.ShowDialog();
                    }
                    else
                    {
                        var quotationBillPriceOptionsWindow = new QuotationsInformationWindows.QuotationBillPriceOptionsWindow()
                        {
                            UserData = this.UserData,
                            QuotationData = quotationData,
                        };
                        quotationBillPriceOptionsWindow.ShowDialog();
                    }
                }
                else
                {
                    CMessageBox.Show($"Access", $"This quotation underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                MessageBoxResult result = CMessageBox.Show("Submit", $"Are you sure to submit \nQ.Code: {QuotationData.QuotationCode}", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        QuotationData.QuotationStatus = Statuses.Submitted.ToString();
                        QuotationData.SubmitDate = DateTime.Now;
                        var query = DatabaseAI.UpdateRecord<Quotation>();
                        connection.Execute(query, QuotationData);
                    }

                    if (status != Statuses.All)
                        QuotationsData.Remove(QuotationData);
                    else
                        StatusChange();
                }
            }
        }
        private void AddToRunning_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                MessageBoxResult result = CMessageBox.Show("Submit", $"Are you sure to work \nQ.Code: {QuotationData.QuotationCode}", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        QuotationData.QuotationStatus = Statuses.Running.ToString();
                        var query = DatabaseAI.UpdateRecord<Quotation>();
                        connection.Execute(query, QuotationData);
                    }
                    if (status != Statuses.All)
                        QuotationsData.Remove(QuotationData);
                    else
                        StatusChange();
                }
            }
        }
        private void AddToHolded_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                MessageBoxResult result = CMessageBox.Show("Submit", $"Are you sure to hold \nQ.Code: {QuotationData.QuotationCode}", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        QuotationData.QuotationStatus = Statuses.Hold.ToString();
                        var query = DatabaseAI.UpdateRecord<Quotation>();
                        connection.Execute(query, QuotationData);
                    }
                    if (status != Statuses.All)
                        QuotationsData.Remove(QuotationData);
                    else
                        StatusChange();
                }
            }
        }
        private void AddToCancel_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation QuotationData)
            {
                MessageBoxResult result = CMessageBox.Show("Submit", $"Are you sure to cancel \nQ.Code: {QuotationData.QuotationCode}", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        QuotationData.QuotationStatus = Statuses.Cancelled.ToString();
                        var query = DatabaseAI.UpdateRecord<Quotation>();
                        connection.Execute(query, QuotationData);
                    }
                    if (status != Statuses.All)
                        QuotationsData.Remove(QuotationData);
                    else
                        StatusChange();
                }
            }
        }
        private void Revise_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                LoadingControl.Visibility = Visibility.Visible;
                Events.DoingEvent.DoEvents();
                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var reviseQuotationData = new Quotation();
                    reviseQuotationData.Update(quotationData);

                    quotationData.QuotationStatus = Statuses.Revision.ToString();
                    query = $"Update [Quotation].[Quotations] Set QuotationStatus = '{quotationData.QuotationStatus}' Where QuotationID = {quotationData.QuotationID}";
                    connection.Execute(query);

                    query = $"Select (Max(QuotationRevise) + 1) as QuotationRevise From [Quotation].[Quotations] " +
                            $"Where QuotationYear = {quotationData.QuotationYear} " +
                            $"And QuotationMonth = {quotationData.QuotationMonth} " +
                            $"And QuotationNumber = {quotationData.QuotationNumber}";

                    reviseQuotationData.QuotationReviseDate = DateTime.Now;
                    reviseQuotationData.QuotationRevise = connection.QueryFirstOrDefault<int>(query);
                    reviseQuotationData.QuotationStatus = Statuses.Running.ToString();
                    reviseQuotationData.QuotationCode = $"{reviseQuotationData.QuotationCode.Substring(0, 17)}/R{reviseQuotationData.QuotationRevise:00}";

                    query = DatabaseAI.InsertRecord<Quotation>();
                    reviseQuotationData.QuotationID = (int)(decimal)connection.ExecuteScalar(query, reviseQuotationData);

                    query = DatabaseAI.GetRecords<Term>($"Where QuotationID = {quotationData.QuotationID} Order By Sort");
                    var terms = connection.Query<Term>(query).ToList();
                    foreach (Term term in terms)
                    {
                        term.QuotationID = reviseQuotationData.QuotationID;
                    }
                    query = DatabaseAI.InsertRecord<Term>();
                    connection.Execute(query, terms);

                    var panels = QPanelController.QuotationPanels(connection, quotationData.QuotationID).ToList();
                    query = DatabaseAI.InsertRecord<QPanel>();
                    if (panels.Count != 0)
                    {
                        foreach (QPanel panelData in panels)
                        {
                            panelData.RevisePanelID = panelData.PanelID;
                            panelData.QuotationID = reviseQuotationData.QuotationID;

                            panelData.PanelID = (int)(decimal)connection.ExecuteScalar(query, panelData);

                            List<QItem> items = QItemController.PanelItems(connection, panelData.RevisePanelID);

                            if (items.Count != 0)
                            {
                                var insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                             $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                             $"Values " +
                                             $"({panelData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                                connection.Execute(insert, items);
                            }
                        }
                    }

                    DeleteFilter_Click(sender, e);
                    RunningQuotations_Click(sender, e);
                }

                LoadingControl.Visibility = Visibility.Collapsed;
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
                if (UserData.QuoteManager)
                {
                    viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, status);
                    YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

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
                if (UserData.QuoteManager)
                {
                    viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, status);
                    YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

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
                if (UserData.QuoteManager)
                {
                    viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, status);
                    YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

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
                if (UserData.QuoteManager)
                {
                    viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, status);
                    YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

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
                if (UserData.QuoteManager)
                {
                    viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, DateTime.Now.Year, status);
                    YearsData = QuotationController.QuotationsYears(connection, status);
                }
                else
                {
                    viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, DateTime.Now.Year, status);
                    YearsData = QuotationController.UserQuotationsYears(connection, UserData.UserID, status);
                }
            }
            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = YearsData;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

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
                    if (UserData.QuoteManager)
                    {
                        viewData.Source = QuotationsData = QuotationController.GetQuotations(connection, yearData.Year, status);
                    }
                    else
                    {
                        viewData.Source = QuotationsData = QuotationController.UserQuotations(connection, UserData.UserID, yearData.Year, status);
                    }
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

            //if (QuotationsList.SelectedItem is Quotation quotationData)
            //{
            //    LoadingControl.Visibility = Visibility.Visible;
            //    Events.DoingEvent.DoEvents();

            //    int row, column, span;
            //    List<DiscountSheetItem> items;

            //    Excel.Application xlApp;
            //    Excel.Workbook xlWorkBook;
            //    Excel.Worksheet xlWorkSheet;

            //    object misValue = Missing.Value;

            //    using (SqlConnection connection = new SqlConnection(DatabaseAI.connectionString))
            //    {
            //        items = DiscountSheetItemController.QuotationItems(connection, quotationData.QuotationID);
            //    }

            //    if (items == null || items.Count == 0)
            //    {
            //        CMessageBox.Show("No Items!!");
            //        return;
            //    }

            //    xlApp = new Excel.Application();
            //    if (xlApp == null)
            //    {
            //        CMessageBox.Show("Excel is not properly installed!!");
            //        return;
            //    }

            //    //xlApp.Visible = true;
            //    //xlApp.WindowState = Excel.XlWindowState.xlMaximized;
            //    //LoadingControl.Visibility = Visibility.Collapsed;

            //    xlWorkBook = xlApp.Workbooks.Add(misValue);
            //    xlWorkSheet = xlWorkBook.Sheets["Sheet1"];
            //    xlWorkSheet.Name = "Circuit breaker";

            //    //Row 3
            //    #region Schneider
            //    xlWorkSheet.Cells[3, 4] = "Schneider";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 4], xlWorkSheet.Cells[3, 8]].Merge();
            //    #endregion

            //    #region Comparison
            //    xlWorkSheet.Cells[3, 9] = ""; //Need Work
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[3, 9], xlWorkSheet.Cells[3, 11]].Merge();
            //    #endregion

            //    //Row 4
            //    #region PPL
            //    xlWorkSheet.Cells[4, 4] = "PPL";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 4], xlWorkSheet.Cells[4, 6]].Merge();
            //    #endregion

            //    #region After Discount
            //    xlWorkSheet.Cells[4, 7] = "After Discount";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 7], xlWorkSheet.Cells[4, 8]].Merge();
            //    #endregion

            //    #region Comparison2
            //    xlWorkSheet.Cells[4, 10] = ""; //Need Work
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[4, 9], xlWorkSheet.Cells[4, 11]].Merge();
            //    #endregion

            //    //Row 5
            //    #region Header
            //    row = 5; column = 1; span = 0;
            //    xlWorkSheet.Cells[row, column] = "SN";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 2; span = 0;
            //    xlWorkSheet.Cells[row, column] = "REF";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 3; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Description";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 4; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Price";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 5; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Qty";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 6; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Total Amount";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 7; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Unit Price";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 8; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Total Price";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 9; span = 0;
            //    xlWorkSheet.Cells[row, column] = "S.C (kA)";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 10; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Unit Price";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    row = 5; column = 11; span = 0;
            //    xlWorkSheet.Cells[row, column] = "Total";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    #endregion

            //    //Row 6
            //    #region Data
            //    var breaker = items.Where(i => i.Article1.ToUpper() == "FEEDER" || i.Article1.ToUpper() == "INCOMER" && i.Brand == "Schneider").OrderBy(i => i.PartNumber).ToList();
            //    for (int i = 0; i < breaker.Count(); i++)
            //    {
            //        row = 6 + i; column = 1; span = 0;
            //        xlWorkSheet.Cells[row, column] = (i + 1).ToString() /*"SN"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 2; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].PartNumber /*REF*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 3; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Description /*"Description"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 4; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Cost /*"Price"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 5; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Qty /*"Qty"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 6; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Qty * breaker[i].Cost /*"Total Amount"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 7; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Cost * (1 - breaker[i].Discount / 100) /*"Unit Price"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 8; span = 0;
            //        xlWorkSheet.Cells[row, column] = breaker[i].Cost * (1 - breaker[i].Discount / 100) * breaker[i].Qty /*"Total Price"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 9; span = 0;
            //        xlWorkSheet.Cells[row, column] = ""/*"S.C (kA)"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 10; span = 0;
            //        xlWorkSheet.Cells[row, column] = "" /*"Unit Price"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 11; span = 0;
            //        xlWorkSheet.Cells[row, column] = "" /*"Total"*/;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    }

            //    #endregion

            //    //Sum
            //    #region Sum
            //    row = 6 + breaker.Count; column = 4; span = 3;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Merge();

            //    column = 8; span = 0;
            //    xlWorkSheet.Cells[row, column].Formula = "=Sum(" + xlWorkSheet.Cells[6, column].Address + ":" + xlWorkSheet.Cells[row - 1, column].Address + ")";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //    column = 9; span = 1;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Merge();

            //    column = 11; span = 0;
            //    xlWorkSheet.Cells[6 + breaker.Count, 11].Formula = "=Sum(" + xlWorkSheet.Cells[6, column].Address + ":" + xlWorkSheet.Cells[row - 1, column].Address + ")";
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //    #endregion

            //    xlWorkSheet.Columns.AutoFit();

            //    var xlSheets = xlWorkBook.Sheets as Excel.Sheets;
            //    xlWorkSheet = (Excel.Worksheet)xlSheets.Add(Type.Missing, xlSheets[1], Type.Missing, Type.Missing);
            //    xlWorkSheet.Name = "Other Material";

            //    var groups = items.Where(i => i.Article1.ToUpper() != "FEEDER" && i.Article1.ToUpper() != "INCOMER" && i.Brand == "Schneider").GroupBy(i => i.Article1).Select(g => g.Key).ToList();
            //    var otherMaterial = items.Where(i => i.Article1.ToUpper() != "FEEDER" && i.Article1.ToUpper() != "INCOMER" && i.Brand == "Schneider").OrderBy(i => i.Article1).OrderBy(i => i.PartNumber).ToList();

            //    row = 3;
            //    foreach (string group in groups)
            //    {
            //        column = 3; span = 3;
            //        xlWorkSheet.Cells[row, column] = group;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + 1, column + span]].Merge();

            //        #region Header
            //        row += 2; column = 1; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Article";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 2; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Reference";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 3; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Description";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 4; span = 0;
            //        xlWorkSheet.Cells[row, column] = "P.P";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 5; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Dis%";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 6; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Dis % Factor";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 7; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Final Price";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 8; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Qty";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 9; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Total";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        #endregion

            //        row++;
            //        var groupCount = otherMaterial.Where(i => i.Article1 == group).Count() - 1;
            //        column = 1; span = 0;
            //        xlWorkSheet.Cells[row, column] = group;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Style.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row + groupCount, column + span]].Merge();

            //        foreach (DiscountSheetItem item in otherMaterial.Where(i => i.Article1 == group).OrderBy(i => i.PartNumber))
            //        {
            //            #region Data
            //            column = 2; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.PartNumber;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 3; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Description;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 4; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Cost;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 5; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Discount + " %";
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 6; span = 0;
            //            xlWorkSheet.Cells[row, column] = (100 - item.Discount) + " %";
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 7; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Cost * (1 - item.Discount / 100);
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 8; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Qty;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //            column = 9; span = 0;
            //            xlWorkSheet.Cells[row, column] = item.Cost * (1 - item.Discount / 100) * item.Qty;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //            #endregion

            //            row++;
            //        }

            //        #region Sum
            //        column = 8; span = 0;
            //        xlWorkSheet.Cells[row, column] = "Total";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            //        column = 9; span = 0;
            //        xlWorkSheet.Cells[row, column].Formula = "=Sum(" + xlWorkSheet.Cells[row - 1 - groupCount, column].Address + ":" + xlWorkSheet.Cells[row - 1, column].Address + ")";
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        xlWorkSheet.Range[xlWorkSheet.Cells[row, column], xlWorkSheet.Cells[row, column + span]].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //        #endregion

            //        row += 2;
            //    }

            //    xlWorkSheet.Columns.AutoFit();

            //    xlApp.Visible = true;
            //    xlApp.WindowState = Excel.XlWindowState.xlMaximized;
            //    LoadingControl.Visibility = Visibility.Collapsed;
            //}




            //Create New Sheets
            //var xlSheets = xlWorkBook.Sheets as Excel.Sheets;
            //var xlNewSheet = (Excel.Worksheet)xlSheets.Add(xlSheets[1], Type.Missing, Type.Missing, Type.Missing);
            //xlNewSheet.Name = "newsheet";

            //Merge()
            //eWSheet.Range[eWSheet.Cells[1, 1], eWSheet.Cells[4, 1]].Merge();

            //border
            //Excel.Range range = xlWorkSheet.UsedRange;
            //Excel.Range cell = range.Cells[1, i];
            //Excel.Borders border = cell.Borders;
            //border[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //border[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //border[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //border[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
        }

        private void QuotationItems_ClicK(object sender, RoutedEventArgs e)
        {
            if(QuotationsList.SelectedItem is Quotation quotationData)
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
                if(pagesNumber != 0)
                {
                    for (int i = 1; i <= pagesNumber; i++)
                    {
                        Printing.QuotationsItems quotationsItems = new Printing.QuotationsItems() { QuotationData = quotationData, Items = items.Where(p => p.ItemSort > ((i - 1) * 45) && p.ItemSort <= ((i) * 45)).ToList(), Page = i, Pages = Convert.ToInt32(pagesNumber) };
                        elements.Add(quotationsItems);
                    }

                    Printing.Print.PrintPreview(elements, $"Quotation {quotationData.QuotationCode} Items");
                }
                else
                {
                    CMessageBox.Show("Items", "There is no items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Revisions_Click(object sender, RoutedEventArgs e)
        {
            if (QuotationsList.SelectedItem is Quotation quotationData)
            {
                if (quotationData.QuotationRevise == 0)
                    return;

                RevisionsWindow revisionsWindow = new RevisionsWindow()
                {
                    UserData = UserData,
                    QuotationData = quotationData
                };
                revisionsWindow.ShowDialog();
            }
        }
    }
}
