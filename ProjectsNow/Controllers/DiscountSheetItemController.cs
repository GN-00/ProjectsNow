using Dapper;
using System.Linq;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    public class DiscountSheetItemController
    {
        public static List<DiscountSheetItem> QuotationItems(SqlConnection connection, int quotationID)
        {
            var query = $"{DatabaseAI.GetFields<DiscountSheetItem>()}" +
                        $"Where QuotationID = {quotationID} " +
                        $"Order By Article1";

            var records = connection.Query<DiscountSheetItem>(query).ToList();
            return records;
        }
    }
}
