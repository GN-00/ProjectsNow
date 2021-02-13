using Dapper;
using System.Linq;
using ProjectsNow.Enums;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class QItemController
    {

		public static void Update(this QItem oldItem, QItem newItem)
		{
			foreach (PropertyInfo property in newItem.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldItem.GetType().GetProperty(property.Name).SetValue(oldItem, newItem.GetType().GetProperty(property.Name).GetValue(newItem));
			}
		}

		public static ObservableCollection<QItem> PanelDetails(SqlConnection connection, int panelID)
		{
			var query = $"Select ItemID, PanelID, Article1, Article2, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [Quotation].[QuotationsPanelsItems] " +
						$"Where PanelID = {panelID} And ItemTable = '{Tables.Details}'" +
						$"Order By ItemSort";

			var items = connection.Query<QItem>(query);
			var records = new ObservableCollection<QItem>(items);
			return records;
		}
		public static ObservableCollection<QItem> PanelEnclosure(SqlConnection connection, int panelID)
		{
			var query = $"Select ItemID, PanelID, Article1, Article2, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [Quotation].[QuotationsPanelsItems] " +
						$"Where PanelID = {panelID} And ItemTable = '{Tables.Enclosure}'" +
						$"Order By ItemSort";

			var items = connection.Query<QItem>(query);
			var records = new ObservableCollection<QItem>(items);
			return records;
		}


		public static List<QItem> PanelItems(SqlConnection connection, int panelID)
		{
			var query = $"Select ItemID, PanelID, Article1, Article2, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [Quotation].[QuotationsPanelsItems] " +
						$"Where PanelID = {panelID} " +
						$"Order By ItemSort";

			var records = connection.Query<QItem>(query).ToList();
			return records;
		}

		public static List<QItem> QuotationRecalculateItems(SqlConnection connection, int quotationID)
		{
			var query = $"Select * " +
						$"From [Quotation].[RecalculateItems] " +
						$"Where QuotationID = {quotationID} " +
						$"Order By ItemSort";

			var records = connection.Query<QItem>(query).ToList();
			return records;
		}

		public static List<QItem> PanelRecalculateItems(SqlConnection connection, int panelID)
		{
			var query = $"Select * " +
						$"From [Quotation].[RecalculateItems] " +
						$"Where PanelID = {panelID} " +
						$"Order By ItemSort";

			var records = connection.Query<QItem>(query).ToList();
			return records;
		}

	}
}
