using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Attributes;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class JPanelController
    {
        public static void Update(this JPanel oldPanel, JPanel newPanel)
        {
            foreach (PropertyInfo property in typeof(JPanel).GetProperties())
            {
                if (property.SetMethod != null)
                    oldPanel.GetType().GetProperty(property.Name).
                    SetValue(oldPanel, typeof(JPanel).GetProperty(property.Name).GetValue(newPanel));
            }
        }

        public static ObservableCollection<JPanel> GetJobOrderPanels(this SqlConnection connection, int jobOrderID)
        {
            string query = $"{DatabaseAI.GetFields<JPanel>()} " +
                           $"Where [JobOrder].[Panels].JobOrderID = {jobOrderID} " +
                           $"Order By [JobOrder].[Panels].PanelSN ";
            var records = connection.Query<JPanel>(query);
            return new ObservableCollection<JPanel>(records);
        }
    }
}
