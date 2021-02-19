using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class SupplierTable : UserControl
    {
        double cm = App.cm;
        public SupplierTable(List<SupplierInvoice> invoices)
        {
            InitializeComponent();

            Border border;
            TextBlock textBlock;

            for (int i = 0; i < invoices.Count; i++)
            {
                this.Height += 0.5 * cm;
                Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5 * cm) });

                //1
                textBlock = new TextBlock()
                {
                    Text = invoices[i].InvoiceDate.ToString("dd/MM/yyyy"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 0);
                Table.Children.Add(border);

                //2
                textBlock = new TextBlock()
                {
                    Text = invoices[i].InvoiceNumber,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 1);
                Table.Children.Add(border);

                //3
                textBlock = new TextBlock()
                {
                    Text = invoices[i].SupplierName,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                if (invoices[i].SupplierName == null) textBlock.Text = Database.DatabaseAI.FactoryStoreName;
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 2);
                Table.Children.Add(border);

                //4
                textBlock = new TextBlock()
                {
                    Text = invoices[i].Amount.ToString("N2"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 3);
                Table.Children.Add(border);

                //5
                textBlock = new TextBlock()
                {
                    Text = invoices[i].VAT.ToString("N2"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 4);
                Table.Children.Add(border);

                //6
                textBlock = new TextBlock()
                {
                    Text = invoices[i].InvoiceTotal.ToString("N2"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 5);
                Table.Children.Add(border);

                //7
                textBlock = new TextBlock()
                {
                    Text = invoices[i].Balance.ToString("N2"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 6);
                Table.Children.Add(border);

                //8
                textBlock = new TextBlock()
                {
                    Text = invoices[i].Status,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                };
                Grid.SetRow(border, i + 2);
                Grid.SetColumn(border, 7);
                Table.Children.Add(border);

            }

            Amount.Text = invoices.Sum(i => i.Amount).ToString("N2");
            VAT.Text = invoices.Sum(i => i.VAT).ToString("N2");
            InvoiceTotal.Text = invoices.Sum(i => i.InvoiceTotal).ToString("N2");
            Balance.Text = invoices.Sum(i => i.Balance).ToString("N2");
        }
        
    }
}

