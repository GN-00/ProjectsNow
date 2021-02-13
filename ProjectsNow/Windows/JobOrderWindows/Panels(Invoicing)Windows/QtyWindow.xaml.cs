using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Invoicing_Windows
{
    public partial class QtyWindow : Window
    {
        public JPanel PanelData { get; set; }
        public Invoice InvoiceData { get; set; }
        public ObservableCollection<TPanel> PanelsTransaction { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PanelToInvoiceInput.Text = PanelData.ReadyToInvoicedQty.ToString();
            DataContext = PanelData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Invoicing_Click(object sender, RoutedEventArgs e)
        {
            int qty = int.Parse(PanelToInvoiceInput.Text);
            if (qty > 0)
            {
                PanelData.InvoicedQty += qty;

                TPanel newPanel = new TPanel()
                {
                    JobOrderID = PanelData.JobOrderID,
                    PanelID = PanelData.PanelID,
                    PanelSN = PanelData.PanelSN,
                    PanelName = PanelData.PanelName,
                    EnclosureType = PanelData.EnclosureType,
                    Qty = qty,
                    Reference = InvoiceData.Number,
                    Date = InvoiceData.Date,
                };
                PanelsTransaction.Add(newPanel);

                this.Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PanelToInvoiceInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToInvoiceInput.Text))
            {
                PanelToInvoiceInput.Text = PanelData.ReadyToInvoicedQty.ToString();
            }
            else
            {
                int qty = int.Parse(PanelToInvoiceInput.Text);
                if (qty > PanelData.ReadyToInvoicedQty)
                    PanelToInvoiceInput.Text = PanelData.ReadyToInvoicedQty.ToString();
            }
        }
        private void PanelToInvoiceInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }

    }
}
