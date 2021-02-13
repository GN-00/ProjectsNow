using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ProjectsNow.Enums;

namespace ProjectsNow.Controllers
{
	public static class QPanelController
	{
		public static void Update(this QPanel oldInformation, QPanel newInfromation)
		{
			foreach (PropertyInfo property in newInfromation.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldInformation.GetType().GetProperty(property.Name).SetValue(oldInformation, newInfromation.GetType().GetProperty(property.Name).GetValue(newInfromation));
			}
		}

		public static ObservableCollection<QPanel> QuotationPanels(SqlConnection connection, int quotationID)
		{
			var query = $"Select * " +
						$"From [Quotation].[QuotationsPanels] " +
						$"LEFT OUTER JOIN [Quotation].[QuotationsPanelsCost] On [QuotationsPanels].PanelID = [QuotationsPanelsCost].PanelID " +
						$"WHERE QuotationID = {quotationID} Order By PanelSN";

			var panels = new ObservableCollection<QPanel>(connection.Query<QPanel>(query));
			return panels;
		}

		public static void UpdateEnclosure(SqlConnection connection, QPanel panelData, Group group, List<List<string>> values, ObservableCollection<QItem> Items)
		{
			if (group.Name == "NSYCRN")
			{
				panelData.EnclosureType = "Universal CRN";
				panelData.EnclosureHeight = double.Parse(values[1][1]);
				panelData.EnclosureWidth = double.Parse(values[1][2]);
				panelData.EnclosureDepth = double.Parse(values[1][3]);

				panelData.EnclosureMetalType = "Steel";
				panelData.EnclosureColor = "7035";
				panelData.EnclosureIP = values[1][4];
				panelData.EnclosureForm = "1";
				panelData.EnclosureLocation = values[1][5] == "With." ? "Outdoor" : "Indoor";
				panelData.EnclosureInstallation = "Wall Mounted";
				panelData.EnclosureFunctional = values[1][6];

				panelData.EnclosureName = $"Universal NSYCRN{panelData.EnclosureHeight}{panelData.Weight}/{panelData.EnclosureDepth} IP{panelData.EnclosureIP} {panelData.EnclosureLocation}";
				string query = DatabaseAI.UpdateRecord<QPanel>();
				connection.Execute(query, panelData);
			}
			else if (group.Name == "NSYSM")
			{
				panelData.EnclosureType = "Universal SM";
				panelData.EnclosureHeight = double.Parse(values[1][1]);
				panelData.EnclosureWidth = double.Parse(values[1][2]);
				panelData.EnclosureDepth = double.Parse(values[1][3]);

				panelData.EnclosureMetalType = "Steel";
				panelData.EnclosureColor = "7035";
				panelData.EnclosureIP = values[1][4];
				panelData.EnclosureForm = "1";
				panelData.EnclosureLocation = values[1][5] == "With." ? "Outdoor" : "Indoor";
				panelData.EnclosureInstallation = "Floor Standing";
				panelData.EnclosureFunctional = values[1][6];

				panelData.EnclosureName = $"Universal NSYSM{panelData.EnclosureHeight}{panelData.Weight}/{panelData.EnclosureDepth} IP{panelData.EnclosureIP} {panelData.EnclosureLocation}";
				string query = DatabaseAI.UpdateRecord<QPanel>();
				connection.Execute(query, panelData);
			}
			else if (group.Name == "Disbo")
			{
				panelData.EnclosureType = $"Disbo Extra";
				panelData.EnclosureHeight = double.Parse(values[2][1]);
				panelData.EnclosureWidth = double.Parse(values[2][2]);
				panelData.EnclosureDepth = double.Parse(values[2][3]);

				panelData.EnclosureMetalType = "Steel";
				panelData.EnclosureColor = "9002";
				panelData.EnclosureIP = values[2][4];
				panelData.EnclosureForm = "1";
				panelData.EnclosureLocation = "Indoor";
				panelData.EnclosureInstallation = $"{values[1][2]} Ways";
				panelData.EnclosureFunctional = "With";

				panelData.EnclosureName = $"Disbo Extra {values[1][2]} Ways {values[1][5]} {values[1][6]}A";
				string query = DatabaseAI.UpdateRecord<QPanel>();
				connection.Execute(query, panelData);

			}
		}

		public static ObservableCollection<QPanel> GetQuotationPanelsWaitingPurcheaseOrder(this SqlConnection connection, int quotationID)
		{
			var query = $"{DatabaseAI.GetFields<QPanel>()}" +
						$"WHERE QuotationsPanels.QuotationID = {quotationID} And QuotationsPanels.PurchaseOrdersNumber Is Null " +
						$"Order By PanelSN";

			var records = new ObservableCollection<QPanel>(connection.Query<QPanel>(query));
			return records;
		}
	}
}
