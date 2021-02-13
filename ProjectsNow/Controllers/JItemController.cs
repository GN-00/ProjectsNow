using Dapper;
using ProjectsNow.Enums;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class JItemController
	{
		public static void Update(this JItem oldItem, JItem newItem)
		{
			foreach (PropertyInfo property in oldItem.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldItem.GetType().GetProperty(property.Name).SetValue(oldItem, oldItem.GetType().GetProperty(property.Name).GetValue(newItem));
			}
		}
		public static ObservableCollection<JItem> PanelDetails(SqlConnection connection, int panelID)
		{
			var query = $"Select PanelID, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [JobOrder].[PanelsItemsView] " +
						$"Where PanelID = {panelID} And ItemTable = '{Tables.Details}'" +
						$"Order By ItemSort";

			var items = connection.Query<JItem>(query);
			var records = new ObservableCollection<JItem>(items);
			return records;
		}
		public static ObservableCollection<JItem> PanelEnclosure(SqlConnection connection, int panelID)
		{
			var query = $"Select PanelID, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [JobOrder].[PanelsItemsView] " +
						$"Where PanelID = {panelID} And ItemTable = '{Tables.Enclosure}'" +
						$"Order By ItemSort";

			var items = connection.Query<JItem>(query);
			var records = new ObservableCollection<JItem>(items);
			return records;
		}
	}
}
