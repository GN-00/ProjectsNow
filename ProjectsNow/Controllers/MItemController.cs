using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class MItemController
    {
        public static void Update(this MItem oldItem, MItem newItem)
        {
            foreach (PropertyInfo property in newItem.GetType().GetProperties())
            {
                if (property.SetMethod != null)
                    oldItem.GetType().GetProperty(property.Name).SetValue(oldItem, newItem.GetType().GetProperty(property.Name).GetValue(newItem));
            }
        }

        public static ObservableCollection<MItem> GetModificationsItems(SqlConnection connection, int jobOrderID)
        {
            var query = $"Select * From [JobOrder].[ModificationsItems] Where JobOrderID = {jobOrderID}";
            var records = new ObservableCollection<MItem>(connection.Query<MItem>(query));
            return records;
        }

        public static List<MItem> GetAllItems(SqlConnection connection, int jobOrderID)
        {
            var query = $"Select * From [JobOrder].[PanelsItemsView] Where JobOrderID = {jobOrderID} Order By JobOrderID, PanelID, ItemTable, ItemSort ";
            var records = connection.Query<MItem>(query).ToList();
            return records;
        }
    }
}
