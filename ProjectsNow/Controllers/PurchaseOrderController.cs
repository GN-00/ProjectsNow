using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class PurchaseOrderController
    {
        public static void Update(this PurchaseOrder oldPurchaseOrder, PurchaseOrder newPurchaseOrder)
        {
            foreach (PropertyInfo property in typeof(PurchaseOrder).GetProperties())
            {
                if (property.SetMethod != null)
                    oldPurchaseOrder.GetType().GetProperty(property.Name).
                    SetValue(oldPurchaseOrder, typeof(PurchaseOrder).GetProperty(property.Name).GetValue(newPurchaseOrder));
            }
        }

        public static ObservableCollection<PurchaseOrder> GetPurchaseOrders(SqlConnection connection, int jobOrderID)
        {
            var query = $"{DatabaseAI.GetFields<PurchaseOrder>()}" +
                        $"Where JobOrderID = {jobOrderID} Order By Date";

            var records = new ObservableCollection<PurchaseOrder>(connection.Query<PurchaseOrder>(query));
            return records;
        }

    }
}
