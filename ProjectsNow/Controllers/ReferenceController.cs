using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    public static class ReferenceController
	{
		public static void Update(this Reference oldReference, Reference newReference)
		{
			foreach (PropertyInfo property in oldReference.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldReference.GetType().GetProperty(property.Name).SetValue(oldReference, oldReference.GetType().GetProperty(property.Name).GetValue(newReference));
			}
		}

		public static List<Reference> GetReferences(SqlConnection connection)
		{
			var query = $"Select ReferenceID, Category, Code, Description, Article1, Article2, Brand, Remarks, " +
						$"Cost, Discount, Unit " +
						$"From [Reference].[References] " +
						$"Where Hide = 0 " +
						$"Order By Category, Code";
			var records = connection.Query<Reference>(query).ToList();
			return records;
		}
		public static List<string> GetArticle1(SqlConnection connection)
		{
			var records = connection.Query<string>("Select Article From [Quotation].[Articles] Order By Sort").ToList();
			return records;
		}
		public static List<string> GetArticle2(SqlConnection connection)
		{
			var records = connection.Query<string>("Select Article2 From [Quotation].[QuotationsPanelsItems] Group By Article2 Order By Article2").ToList();
			return records;
		}
	}
}
