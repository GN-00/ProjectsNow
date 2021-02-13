using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class ConsultantController
    {
		public static void Update(this Consultant oldConsultant, Consultant newConsultant)
		{
			foreach (PropertyInfo property in newConsultant.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldConsultant.GetType().GetProperty(property.Name).SetValue(oldConsultant, newConsultant.GetType().GetProperty(property.Name).GetValue(newConsultant));
			}
		}

		public static ObservableCollection<Consultant> GetConsultants(SqlConnection connection)
		{
			var query = DatabaseAI.GetFields<Consultant>() +
						$"Order By ConsultantName";
			var records = new ObservableCollection<Consultant>(connection.Query<Consultant>(query));
			return records;
		}

		public static bool AbilityToDelete(this Consultant consultant, SqlConnection connection)
		{
			var query = $"Select ConsultantID " +
						$"From [Inquiries] " +
						$"Where ConsultantID = {consultant.ConsultantID} ";
			var records = connection.Query<Consultant>(query).ToList();
			return (records.Count == 0 ? true : false);
		}
	}
}
