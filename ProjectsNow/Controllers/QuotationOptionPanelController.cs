using Dapper;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class QuotationOptionPanelController
    {
        public static ObservableCollection<QuotationOptionPanel> GetPanels(SqlConnection connection, string optionsIDs)
        {
            var query = $"{DatabaseAI.GetFields<QuotationOptionPanel>()}" +
                        $"WHERE QuotationsOptionsPanels.OptionID In ({optionsIDs}) " +
                        $"Order By QuotationsPanels.PanelSN";

            var records = new ObservableCollection<QuotationOptionPanel>(connection.Query<QuotationOptionPanel>(query));
            return records;
        }

        public static ObservableCollection<QuotationOptionPanel> GetQuotationPanels(SqlConnection connection, int quotationID, int optionID)
        {
            var query = $"Select QuotationsPanels.PanelID, QuotationsPanels.PanelSN, QuotationsPanels.PanelName, QuotationsPanels.PanelQty, QuotationsPanels.EnclosureName " +
                        $"From [Quotation].[QuotationsPanels] " +
                        $"Left Outer Join [Quotation].[QuotationsOptionsPanels] On QuotationsPanels.PanelID = QuotationsOptionsPanels.PanelID And QuotationsOptionsPanels.OptionID = {optionID} " +
                        $"WHERE (QuotationsPanels.QuotationID = {quotationID}) And (QuotationsOptionsPanels.ID IS NULL) " +
                        $"Order By QuotationsPanels.PanelSN";

            var records = new ObservableCollection<QuotationOptionPanel>(connection.Query<QuotationOptionPanel>(query));
            return records;
        }
    }
}
