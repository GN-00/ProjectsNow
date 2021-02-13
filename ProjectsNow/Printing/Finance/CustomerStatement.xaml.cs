using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using ProjectsNow.Windows.FinanceWindows.CustomersWindows;

namespace ProjectsNow.Printing.Finance
{
    public partial class CustomerStatement : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public StatementInformation Info { get; set; }
        public List<Statement> Statements { get; set; }

        double cm = App.cm;
        public CustomerStatement()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;

            PageView.Text = Page.ToString();
            PagesView.Text = Pages.ToString();
            if (Page == Pages) TotalArea.Visibility = Visibility.Visible;

            DataContext = Info;

            int row = 0;
            Border border;
            TextBlock textBlock;
            foreach (Statement statement in Statements)
            {
                row++;
                Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.6 * cm) });

                //Column 1
                textBlock = new TextBlock()
                {
                    Text = statement.Date.ToString("dd/MM/yyyy"),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1)
                };
                border.Child = textBlock;
                Table.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 0);

                //Column 2
                textBlock = new TextBlock()
                {
                    Text = statement.Description,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                border.Child = textBlock;
                Table.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 1);

                //Column 3
                textBlock = new TextBlock()
                {
                    Text = statement.Debit.ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                border.Child = textBlock;
                Table.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 2);

                //Column 4
                textBlock = new TextBlock()
                {
                    Text = statement.Credit.ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                border.Child = textBlock;
                Table.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 3);

                //Column 5
                textBlock = new TextBlock()
                {
                    Text = statement.Balance.ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                border.Child = textBlock;
                Table.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 4);

            }
        }


    }
}
