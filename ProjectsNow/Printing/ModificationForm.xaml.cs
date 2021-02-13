using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace ProjectsNow.Printing
{
    public partial class ModificationForm : UserControl
    {
        public int PanelID { get; set; }
        public MItem ItemData { get; set; }
        public Actions ActionData { get; set; }
        public Modification ModificationData { get; set; }
        public ObservableCollection<MItem> ItemsData { get; set; }

        public ModificationForm()
        {
            InitializeComponent();
        }
    }
}
