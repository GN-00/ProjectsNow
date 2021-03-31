using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class TermWindow : Window
    {
        public User UserData { get; set; }
        public Term TermData { get; set; }
        public Actions ActionsData { get; set; }
        public TermsAndConditionsWindow TermsAndConditionsWindowData { get; set; }

        string conditionType;
        public TermWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (ActionsData == Actions.New)
            {
                var selectedConditionTab = TermsAndConditionsWindowData.Tabs.SelectedIndex;
                conditionType = ((ConditionTypes)selectedConditionTab).ToString();
            }
            else
            {
                conditionType = TermData.ConditionType;
                Condition.Text = TermData.Condition;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Term term;
            if (string.IsNullOrWhiteSpace(Condition.Text))
            {
                CMessageBox.Show("Error", "Please write correct term!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
            else
            {
                if (ActionsData == Actions.New)
                {
                    term = new Term()
                    {
                        QuotationID = TermsAndConditionsWindowData.QuotationData.QuotationID,
                        Condition = Condition.Text,
                        ConditionType = conditionType,
                        IsUsed = true,
                        IsDefault = false,
                        Sort = TermsAndConditionsWindowData.terms.Where<Term>(item => item.ConditionType == conditionType).Max<Term>(item => item.Sort) + 1,
                    };

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        var query = DatabaseAI.InsertRecord<Term>();
                        term.TermID = (int)(decimal)connection.ExecuteScalar(query, term);
                    }

                    TermsAndConditionsWindowData.terms.Add(term);
                    var listBoxName = conditionType;
                    var listBox = TermsAndConditionsWindowData.FindName(listBoxName) as ListBox;
                    listBox.ItemsSource = TermsAndConditionsWindowData.terms.Where<Term>(item => item.ConditionType == conditionType);

                }
                else
                {
                    TermData.Condition = Condition.Text;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        var query = DatabaseAI.UpdateRecord<Term>();
                        connection.Execute(query, TermData);
                    }
                }

                this.Close();

            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Condition_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.V)
                Condition.Text.Insert(Condition.CaretIndex, Clipboard.GetText());
        }
    }
}
