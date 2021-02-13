using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;

namespace ProjectsNow.Printing
{
    public partial class QuotationCoverPage : UserControl
    {
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }
        public List<QuotationContent> Contents { get; set; }

        Contact ContactData { get; set; }

        public QuotationCoverPage()
        {
            InitializeComponent();
        }

        public QuotationCoverPage(User user, Quotation quotation, List<QuotationContent> contents)
        {
            InitializeComponent();
            UserData = user;
            QuotationData = quotation;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                ContactData = ContactController.GetProjectAttention(connection, QuotationData.InquiryID);
            }

            Contents = contents;
            DataContext = new { UserData, QuotationData, ContactData, Contents };
        }
    }
}
