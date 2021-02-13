using Dapper;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class MPanelController
    {
        public static ObservableCollection<MPanel> GetPanels(SqlConnection connection, int jobOrderID)
        {
            var query = $"Select * From [JobOrder].[Panels(Running)] Where JobOrderID = {jobOrderID}";
            var records = new ObservableCollection<MPanel>(connection.Query<MPanel>(query));
            return records;
        }
    }
}
