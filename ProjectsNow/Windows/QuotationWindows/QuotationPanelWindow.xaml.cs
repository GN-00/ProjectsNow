using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QPanelWindow : Window
    {
        public int SelectedIndex { get; set; }
        public Actions ActionData { get; set; }
        public QPanel QPanelData { get; set; }
        public Quotation QuotationData { get; set; }
        public ObservableCollection<QPanel> QPanelsData { get; set; }


        QPanel newQPanelData;
        public QPanelWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData != Actions.Edit)
            {
                newQPanelData = new QPanel()
                {
                    Source = QuotationData.PowerVoltage,
                    Frequency = QuotationData.Frequency,
                    Busbar = QuotationData.TinPlating,
                    NeutralSize = QuotationData.NeutralSize,
                    EarthSize = QuotationData.EarthSize,
                    EarthingSystem = QuotationData.EarthingSystem,
                    QuotationID = QuotationData.QuotationID,
                };
            }
            else
            {
                newQPanelData = new QPanel();
                newQPanelData.Update(QPanelData);
            }

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                EnclosureType.ItemsSource = connection.Query("Select EnclosureType From [Quotation].[QuotationsPanels] Where EnclosureType Is Not Null Group By EnclosureType");
                EnclosureMetalType.ItemsSource = connection.Query("Select EnclosureMetalType From [Quotation].[QuotationsPanels] Where EnclosureMetalType Is Not Null Group By EnclosureMetalType ");
                EnclosureColor.ItemsSource = connection.Query("Select EnclosureColor From [Quotation].[QuotationsPanels] Where EnclosureColor Is Not Null Group By EnclosureColor");
                EnclosureIP.ItemsSource = connection.Query("Select EnclosureIP From [Quotation].[QuotationsPanels] Where EnclosureIP Is Not Null Group By EnclosureIP");
                EnclosureForm.ItemsSource = connection.Query("Select EnclosureForm From [Quotation].[QuotationsPanels] Where EnclosureForm Is Not Null Group By EnclosureForm ");
                EnclosureFunctional.ItemsSource = connection.Query("Select EnclosureFunctional From [Quotation].[QuotationsPanels] Where EnclosureFunctional Is Not Null Group By EnclosureFunctional");
                EnclosureDoor.ItemsSource = connection.Query("Select EnclosureDoor From [Quotation].[QuotationsPanels] Where EnclosureDoor Is Not Null Group By EnclosureDoor");
                Source.ItemsSource = connection.Query("Select Source From [Quotation].[QuotationsPanels] Where Source Is Not Null Group By Source");
            }

            DataContext = new { newQPanelData };
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
            QPanel checkName;
            if (ActionData == Actions.New) checkName = QPanelsData.Where(p => p.PanelName == newQPanelData.PanelName).FirstOrDefault();
            else checkName = QPanelsData.Where(p => p.PanelName == newQPanelData.PanelName && p.PanelID != QPanelData.PanelID).FirstOrDefault();

            if (checkName != null)
            {
                CMessageBox.Show("Name Error", $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            bool isReady = true;
            string message = "Please Enter:";
            string query;
            if (newQPanelData.PanelName == null || newQPanelData.PanelName == "") { message += $"\n Panel Name."; isReady = false; }
            if (newQPanelData.PanelQty == 0) { message += $"\n Panel Qty."; isReady = false; }
            if (newQPanelData.PanelType == null || newQPanelData.PanelType == "") { message += $"\n Panel Type."; isReady = false; }

            if (isReady == true)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    if (ActionData == Actions.Edit)
                    {
                        query = DatabaseAI.UpdateRecord<QPanel>();
                        connection.Execute(query, newQPanelData);
                        QPanelData.Update(newQPanelData);
                    }
                    else
                    {
                        newQPanelData.PanelSN = SelectedIndex + 1;

                        if (ActionData == Actions.New)
                        {
                            QPanelsData.Add(newQPanelData);
                        }
                        else
                        {
                            connection.Execute($"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN + 1 Where PanelSN >= {newQPanelData.PanelSN} and QuotationID ={newQPanelData.QuotationID}");
                            foreach (QPanel panel in QPanelsData.Where(panel => panel.PanelSN >= newQPanelData.PanelSN))
                            {
                                panel.PanelSN++;
                            }
                            QPanelsData.Insert(SelectedIndex, newQPanelData);
                        }

                        query = DatabaseAI.InsertRecord<QPanel>();
                        newQPanelData.PanelID = (int)(decimal)connection.ExecuteScalar(query, newQPanelData);
                    }
                }

                QuotationData.QuotationCost = QPanelsData.Sum(p => p.PanelsPrice);
                this.Close();
            }
            else
            {
                CMessageBox.Show("Missing Data", message, CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void IntOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
        private void DoubleOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void QTY_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "" || textBox.Text == null || textBox.Text == "0")
            {
                newQPanelData.PanelQty = 1;
                textBox.Text = "1";
            }
        }
        private void Profit_Discount_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            if (text == null || text == "")
                ((TextBox)sender).Text = "0";
            else
            {
                double value = double.Parse(text);
                if (value > 100)
                {
                    ((TextBox)sender).Text = (100).ToString();
                }
                else if (value < 0)
                {
                    ((TextBox)sender).Text = (0).ToString();
                }
                else
                {
                    ((TextBox)sender).Text = (value).ToString();
                }
            }

        }

    }
}
