using Dapper;
using System.Linq;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    class BillItemController
    {
		public static List<BillItem> PanelsDetails(SqlConnection connection, string panelsID)
		{
			var query = $"Select ItemID, PanelID, Article1, Article2, Category, Code, Description, " +
						$"Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
						$"From [Quotation].[QuotationsPanelsItems] " +
						$"Where PanelID In ({panelsID}) And ItemTable = '{Tables.Details}'" +
						$"Order By PanelID, ItemSort";

			var records = connection.Query<BillItem>(query).ToList();
			return records;
		}
	}
}
