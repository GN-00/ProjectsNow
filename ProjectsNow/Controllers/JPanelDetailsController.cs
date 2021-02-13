using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Attributes;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class JPanelDetailsController
    {
        public static void Update(this JPanelDetails oldPanel, JPanelDetails newPanel)
        {
            foreach (PropertyInfo property in typeof(JPanelDetails).GetProperties())
            {
                if (property.SetMethod != null)
                    oldPanel.GetType().GetProperty(property.Name).
                    SetValue(oldPanel, typeof(JPanelDetails).GetProperty(property.Name).GetValue(newPanel));
            }
        }

        public static void Update(this JPanelDetails JPanel, QPanel QPanel)
        {
            foreach (PropertyInfo property in typeof(JPanelDetails).GetProperties())
            {
                var attributes = (QPanelProperty)typeof(JPanelDetails).GetProperty(property.Name).GetCustomAttribute(typeof(QPanelProperty));
                if (attributes != null)
                {
                    if (property.SetMethod != null)
                        JPanel.GetType().GetProperty(property.Name).
                        SetValue(JPanel, typeof(QPanel).GetProperty(property.Name).GetValue(QPanel));
                }
            }
        }

        public static ObservableCollection<JPanelDetails> GetJobOrderPanels(this SqlConnection connection, int jobOrderID)
        {
            string query = $"{DatabaseAI.GetFields<JPanelDetails>()} " +
                           $"Where [JobOrder].[Panels].JobOrderID = {jobOrderID} " +
                           $"Order By [JobOrder].[Panels].PanelSN ";
            var records = connection.Query<JPanelDetails>(query);
            return new ObservableCollection<JPanelDetails>(records);
        }

        public static JPanelDetails GetPanel(this SqlConnection connection, int panelID)
        {
            string query = $"{DatabaseAI.GetFields<JPanelDetails>()} " +
                           $"Where [JobOrder].[Panels].PanelID = {panelID} ";
            var record = connection.QueryFirstOrDefault<JPanelDetails>(query);
            return record;
        }
    }
}
