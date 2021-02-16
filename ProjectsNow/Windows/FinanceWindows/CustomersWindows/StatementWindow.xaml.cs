using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;


namespace ProjectsNow.Windows.FinanceWindows.CustomersWindows
{
    public partial class StatementWindow : Window
    {
        public User UserData { get; set; }
        public CustomerInformation CustomerInformation { get; set; }

        DateTime? stratDate;
        DateTime? endDate;
        List<Statement> debit;
        List<Statement> credit;
        List<Statement> statements;

        public StatementWindow()
        {
            InitializeComponent();
            CustomerInformation = new CustomerInformation() { ID = 12 };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            double balanceLastYears;
            List<Statement> debitLastYears;
            List<Statement> creditLastYears;

            if (StartDatePicker.SelectedDate is DateTime firstDate) stratDate = firstDate;
            else stratDate = new DateTime(DateTime.Now.Year, 1, 1);

            if (EndDatePicker.SelectedDate is DateTime lastDate) endDate = lastDate;
            else endDate = new DateTime(DateTime.Now.Year, 12, 31);

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Finance].[CustomersDebit(Details)] Where CustomerID = {CustomerInformation.ID} And Date >= @stratDate And Date <= @endDate";
                debit = connection.Query<Statement>(query, new { stratDate, endDate }).ToList();

                query = $"Select * From [Finance].[CustomersCredit(Details)] Where CustomerID = {CustomerInformation.ID} And Date >= @stratDate And Date <= @endDate";
                credit = connection.Query<Statement>(query, new { stratDate, endDate }).ToList();

                query = $"Select * From [Finance].[CustomersDebit(Details)] Where CustomerID = {CustomerInformation.ID} And Date < @stratDate";
                debitLastYears = connection.Query<Statement>(query, new { stratDate }).ToList();

                query = $"Select * From [Finance].[CustomersCredit(Details)] Where CustomerID = {CustomerInformation.ID} And Date < @stratDate";
                creditLastYears = connection.Query<Statement>(query, new { stratDate }).ToList();
            }

            balanceLastYears = debitLastYears.Sum(s => s.Debit).GetValueOrDefault() + creditLastYears.Sum(s => s.Credit).GetValueOrDefault();

            statements = new List<Statement>();
            statements.AddRange(debit);
            statements.AddRange(credit);
            statements.Sort((x, y) => x.Date.CompareTo(y.Date));

            if (stratDate.GetValueOrDefault().Day == 1 && stratDate.GetValueOrDefault().Month == 1)
                statements.Insert(0, new Statement() { Date = stratDate.GetValueOrDefault(), Description = "Last Years Closing Balance", Balance = balanceLastYears });
            else
                statements.Insert(0, new Statement() { Date = stratDate.GetValueOrDefault(), Description = "Previous Balance", Balance = balanceLastYears });

            double balance = 0;
            foreach (Statement statement in statements)
            {
                statement.SN = statements.IndexOf(statement) + 1;
                statement.Balance = balance = balance + statement.Credit.GetValueOrDefault() - statement.Debit.GetValueOrDefault();
            }

            StartDatePicker.SelectedDate = stratDate;
            EndDatePicker.SelectedDate = endDate;

            CustomerName.Text = CustomerInformation.CustomerName;
            TotalCredit.Text = statements.Sum(s => s.Credit).GetValueOrDefault().ToString("N2");
            TotalDebit.Text = statements.Sum(s => s.Debit).GetValueOrDefault().ToString("N2");
            ClosingBalance.Text = (statements.Sum(s => s.Credit).GetValueOrDefault() - statements.Sum(s => s.Debit).GetValueOrDefault()).ToString("N2");
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = statements };
            viewData.Filter += DataFilter;

            StatementsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (statements.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
            var selectedIndex = StatementsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Customers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Customer: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void StatementsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = StatementsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Customers: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Customer: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(Statement).GetProperty("Date"),
            typeof(Statement).GetProperty("Describtion"),
            typeof(Statement).GetProperty("Debit"),
            typeof(Statement).GetProperty("Credit"),
            typeof(Statement).GetProperty("BalanceView"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is CustomerInformation record)
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
        private void Print_ClicK(object sender, RoutedEventArgs e)
        {
            const double rows = 32;
            double pages = statements.Count / rows;
            if (pages != Convert.ToInt32(pages)) pages = Convert.ToInt32(pages) + 1;

            StatementInformation Info = new StatementInformation()
            {
                CustomerName = CustomerInformation.CustomerName,
                StartDate = stratDate.GetValueOrDefault(),
                EndDate = endDate.GetValueOrDefault(),
                VATNumber = CustomerInformation.VATNumber,
                Debit = statements.Sum(s => s.Debit).GetValueOrDefault(),
                Credit = statements.Sum(s => s.Credit).GetValueOrDefault(),
            };
            
            List<FrameworkElement> elements = new List<FrameworkElement>();
            if (pages != 0)
            {
                for (int i = 1; i <= pages; i++)
                {
                    Printing.Finance.CustomerStatement statement = new Printing.Finance.CustomerStatement()
                    {
                        Info = Info,
                        Statements = statements.Where(s => s.SN > ((i - 1) * rows) && s.SN <= ((i) * rows)).ToList(),
                        Page = i,
                        Pages = Convert.ToInt32(pages) 
                    };
                    elements.Add(statement);
                }

                Printing.Print.PrintPreview(elements, $"Statement-{Info.CustomerName}");
            }
            else
            {
                CMessageBox.Show("Statement", "There is no items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }
        private void Apply_ClicK(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate is DateTime firstDate) stratDate = firstDate;
            else stratDate = statements.Min(s => s.Date);

            if (EndDatePicker.SelectedDate is DateTime lastDate) stratDate = lastDate;
            else endDate = statements.Max(s => s.Date);

            Window_Loaded(sender, e);
        }
    }
}
