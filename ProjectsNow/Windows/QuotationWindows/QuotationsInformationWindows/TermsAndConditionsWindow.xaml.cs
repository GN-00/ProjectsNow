using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class TermsAndConditionsWindow : Window
    {
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }

        public TermsAndConditionsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Term> terms;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                var query = DatabaseAI.GetRecords<Term>($"Where QuotationID = {QuotationData.QuotationID} Order By Sort");
                terms = new ObservableCollection<Term>(connection.Query<Term>(query));
            }

            ScopeOfSupply.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.ScopeOfSupply.ToString()).OrderBy(t => t.Sort);
            TotalPrice.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.TotalPrice.ToString()).OrderBy(t => t.Sort);
            PaymentConditions.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.PaymentConditions.ToString()).OrderBy(t => t.Sort);
            ValidityPeriod.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.ValidityPeriod.ToString()).OrderBy(t => t.Sort);
            ShopDrawingSubmittals.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.ShopDrawingSubmittals.ToString()).OrderBy(t => t.Sort);
            Delivery.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.Delivery.ToString()).OrderBy(t => t.Sort);
            Guarantee.ItemsSource = terms.Where<Term>(term => term.ConditionType == ConditionTypes.Guarantee.ToString()).OrderBy(t => t.Sort);

            DataContext = new { QuotationData };

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
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
