using System.Reflection;
using ProjectsNow.Database;

namespace ProjectsNow.Controllers
{
    public static class CompanyPOTransactionController
    {
		public static void Update(this ItemPurchased oldInformation, ItemPurchased newInfromation)
		{
			foreach (PropertyInfo property in newInfromation.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldInformation.GetType().GetProperty(property.Name).SetValue(oldInformation, newInfromation.GetType().GetProperty(property.Name).GetValue(newInfromation));
			}
		}
	}
}
