using ProjectsNow.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelProgressionWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }

        public PanelProgressionWindow()
        {
            InitializeComponent();
        }
    }
}
