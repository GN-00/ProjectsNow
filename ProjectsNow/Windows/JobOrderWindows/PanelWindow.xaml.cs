using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Printing;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }


        JPanelDetails newPanelData;
        bool isPrinting = false;
        bool isReadyToPrint = true;
        public PanelWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                newPanelData = JPanelDetailsController.GetPanel(connection, PanelData.PanelID);
                EnclosureType.ItemsSource = connection.Query("Select EnclosureType From [Quotation].[QuotationsPanels] Where EnclosureType Is Not Null Group By EnclosureType");
                EnclosureMetalType.ItemsSource = connection.Query("Select EnclosureMetalType From [Quotation].[QuotationsPanels] Where EnclosureMetalType Is Not Null Group By EnclosureMetalType ");
                EnclosureColor.ItemsSource = connection.Query("Select EnclosureColor From [Quotation].[QuotationsPanels] Where EnclosureColor Is Not Null Group By EnclosureColor");
                EnclosureIP.ItemsSource = connection.Query("Select EnclosureIP From [Quotation].[QuotationsPanels] Where EnclosureIP Is Not Null Group By EnclosureIP");
                EnclosureForm.ItemsSource = connection.Query("Select EnclosureForm From [Quotation].[QuotationsPanels] Where EnclosureForm Is Not Null Group By EnclosureForm ");
                EnclosureFunctional.ItemsSource = connection.Query("Select EnclosureFunctional From [Quotation].[QuotationsPanels] Where EnclosureFunctional Is Not Null Group By EnclosureFunctional");
                EnclosureDoor.ItemsSource = connection.Query("Select EnclosureDoor From [Quotation].[QuotationsPanels] Where EnclosureDoor Is Not Null Group By EnclosureDoor");
                Source.ItemsSource = connection.Query("Select Source From [Quotation].[QuotationsPanels] Where Source Is Not Null Group By Source");
            }

            DataContext = new { newPanelData };
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
            JPanel checkName;
            checkName = PanelsData.Where(p => p.PanelName == newPanelData.PanelName && p.PanelID != PanelData.PanelID).FirstOrDefault();
            if (checkName != null)
            {
                CMessageBox.Show("Name Error", $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                return;
            }

            bool isReady = true;
            string message = "Please Enter:";
            string query;
            if (newPanelData.PanelName == null || newPanelData.PanelName == "") { message += $"\n Panel Name."; isReady = false; }
            if (newPanelData.PanelQty == 0) { message += $"\n Panel Qty."; isReady = false; }
            if (newPanelData.PanelType == null || newPanelData.PanelType == "") { message += $"\n Panel Type."; isReady = false; }

            if (isReady == true)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = DatabaseAI.UpdateRecord<JPanelDetails>();
                    connection.Execute(query, newPanelData);

                    PanelData.PanelName = newPanelData.PanelName;
                    PanelData.EnclosureType = newPanelData.EnclosureType;
                    PanelData.EnclosureHeight = newPanelData.EnclosureHeight;
                    PanelData.EnclosureWidth = newPanelData.EnclosureWidth;
                    PanelData.EnclosureDepth = newPanelData.EnclosureDepth;
                    PanelData.EnclosureIP = newPanelData.EnclosureIP;
                }

                if (isPrinting == false)
                    this.Close();
                else
                    isReadyToPrint = true;
            }
            else
            {
                CMessageBox.Show("Missing Data", message, CMessageBoxButton.OK, CMessageBoxImage.Warning);
                if (isPrinting == true)
                    isReadyToPrint = false;
            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DoubleOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            bool needSave = false;
            foreach(PropertyInfo property in typeof(JPanel).GetProperties())
            {
                var oldData = typeof(JPanel).GetProperty(property.Name).GetValue(PanelData);
                var newData = typeof(JPanel).GetProperty(property.Name).GetValue(newPanelData);
                if(oldData == null && newData != null || oldData != null && newData == null)
                {
                    needSave = true;
                    break;
                }
                else if(oldData != null && newData != null)
                {
                    if (!oldData.Equals(newData))
                    {
                        needSave = true;
                        break;
                    }
                }
            }

            if (needSave)
            {
                isPrinting = true;
                MessageBoxResult result = CMessageBox.Show("Saving", "Are you sure want to Save changes?!", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                if(result == MessageBoxResult.No)
                {
                    isReadyToPrint = false;
                    return;
                }
                Save_Click(sender, e);
                isPrinting = false;
            }

            if(isReadyToPrint == true)
            {
                FrameworkElement page = new CheckList() { PanelData = this.newPanelData, JobOrderData = this.JobOrderData };
                Print.PrintPreview(page, $"{JobOrderData.Code}-{PanelData.PanelSN}-{PanelData.PanelName}-CheckList");
            }
        }
    }
}
