using System.Windows;
using ProjectsNow.Database;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ProjectsNow.Printing
{
    public partial class DeliveryForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public List<DPanel> PanelsData { get; set; }
        public DeliveryInfromation DeliveryInfromation { get; set; }
        public DeliveryForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;

            double rowHeight = 22.6771653543307;
            foreach (RowDefinition row in DeliveryTable.RowDefinitions)
                row.Height = new GridLength(0);

            DeliveryTable.RowDefinitions[0].Height = new GridLength(rowHeight);

            for (int i = 1; i <= PanelsData.Count; i++)
                DeliveryTable.RowDefinitions[i].Height = new GridLength(rowHeight);

            DataContext = new { PanelsData, DeliveryInfromation, Page, Pages };
        }

    }
}
