using Dapper;
using System.Linq;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
	public class BillPanelController
	{
		public static List<BillPanel> GetBillPanels(SqlConnection connection, int quotationID)
		{
			var query = $"Select QuotationsPanels.PanelID, QuotationID, PanelSN, PanelName, PanelQty, PanelProfit, PanelCost, " +
						$"EnclosureType, EnclosureInstallation, EnclosureLocation, EnclosureHeight, EnclosureWidth, EnclosureDepth, " +
						$"EnclosureIP, Source, Frequency, EarthingSystem " +
						$"From [Quotation].[QuotationsPanels] " +
						$"LEFT OUTER JOIN [Quotation].[QuotationsPanelsCost] On QuotationsPanels.PanelID = QuotationsPanelsCost.PanelID " +
						$"Where QuotationID = {quotationID} Order By PanelSN";

			var panels = connection.Query<BillPanel>(query).ToList();
			return panels;
		}

		public static List<BillPanel> GetBillPanels(SqlConnection connection, string panelsIDs)
		{
			var query = $"Select QuotationsPanels.PanelID, QuotationsPanels.QuotationID, QuotationsPanels.PanelSN, " +
						$"QuotationsPanels.PanelName, QuotationsPanels.PanelQty, QuotationsPanels.PanelProfit, QPanelsCost.PanelCost, " +
						$"QuotationsPanels.EnclosureType, QuotationsPanels.EnclosureInstallation, QuotationsPanels.EnclosureLocation, " +
						$"QuotationsPanels.EnclosureHeight, QuotationsPanels.EnclosureWidth, QuotationsPanels.EnclosureDepth, " +
						$"QuotationsPanels.EnclosureIP, QuotationsPanels.Source, QuotationsPanels.Frequency, QuotationsPanels.EarthingSystem " +
						$"From [Quotation].[QuotationsPanels] " +
						$"LEFT OUTER JOIN [Quotation].[QuotationsPanelsCost] On QuotationsPanels.PanelID = QuotationsPanelsCost.PanelID " +
						$"Where QuotationsPanels.PanelID In({panelsIDs}) " +
						$"Order By QuotationsPanels.PanelSN";

			var panels = connection.Query<BillPanel>(query).ToList();
			return panels;
		}
	}
}
