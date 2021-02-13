using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class TermsAndConditionsWindow : Window
    {
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }

        ListBox listBox;
        public ObservableCollection<Term> terms;

        public TermsAndConditionsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                var query = DatabaseAI.UpdateRecords<Term>();
                connection.Execute(query, terms);
            }
            CloseWindow_Click(sender, e);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow_Click(sender, e);
        }

        private void ListBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            listBox = ((ListBox)sender);
            if (e.OriginalSource.GetType() != typeof(ScrollViewer))
            {
                if (listBox.SelectedItem is Term term)
                {
                    if (term.IsDefault)
                        listBox.ContextMenu = this.Resources["DefaultItemContextMenu"] as ContextMenu;
                    else
                        listBox.ContextMenu = this.Resources["ItemContextMenu"] as ContextMenu;
                }
                else
                {
                    listBox.ContextMenu = this.Resources["NoItemContextMenu"] as ContextMenu;
                }
            }
            else
            {
                listBox.ContextMenu = this.Resources["NoItemContextMenu"] as ContextMenu;
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            TermWindow termWindow = new TermWindow()
            {
                UserData = this.UserData,
                ActionsData = Actions.New,
                TermsAndConditionsWindowData = this,
            };
            termWindow.ShowDialog();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (listBox != null)
            {
                if (listBox.SelectedItem is Term term)
                {
                    TermWindow termWindow = new TermWindow()
                    {
                        UserData = this.UserData,
                        ActionsData = Actions.Edit,
                        TermsAndConditionsWindowData = this,
                        TermData = term,
                    };
                    termWindow.ShowDialog();
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (listBox != null)
            {
                if (listBox.SelectedItem is Term term)
                {
                    if (!term.IsDefault)
                    {
                        MessageBoxResult result = CMessageBox.Show("Deleting", "Are you Sure to delete this term?", CMessageBoxButton.YesNo, CMessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                            {
                                var query = DatabaseAI.DeleteRecord<Term>(term.TermID);
                                connection.Execute(query);
                            }

                            var conditionType = term.ConditionType;
                            terms.Remove(term);

                            var listBoxName = conditionType;
                            var listBox = FindName(listBoxName) as ListBox;
                            listBox.ItemsSource = terms.Where<Term>(item => item.ConditionType == conditionType);
                        }
                    }
                }
            }
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem is Term term)
                Clipboard.SetText(term.Condition);
        }
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (listBox != null)
            {
                if (listBox.SelectedItem is Term term)
                {
                    if (term.Sort > 1)
                    {
                        term.Sort--;
                        foreach (Term termData in terms.Where(t => t.Sort == term.Sort && t.ConditionType == term.ConditionType && t.TermID != term.TermID))
                        {
                            ++termData.Sort;
                        }
                        ((ListBox)FindName(term.ConditionType)).ItemsSource = terms.Where(t => t.ConditionType == term.ConditionType).OrderBy(t => t.Sort);
                    }
                }
            }
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (listBox != null)
            {
                if (listBox.SelectedItem is Term term)
                {
                    int listCont = terms.Where(t => t.ConditionType == term.ConditionType).Count();
                    if (term.Sort < listCont && listCont != 1)
                    {
                        term.Sort++;
                        foreach (Term termData in terms.Where(t => t.Sort == term.Sort && t.ConditionType == term.ConditionType && t.TermID != term.TermID))
                        {
                            --termData.Sort;
                        }
                        ((ListBox)FindName(term.ConditionType)).ItemsSource = terms.Where(t => t.ConditionType == term.ConditionType).OrderBy(t => t.Sort);

                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.QuotationID = null;
                UserController.UpdateQuotationID(connection, UserData);
            }
        }
    }
}
