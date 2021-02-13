using System.Reflection;
using ProjectsNow.Database;

namespace ProjectsNow.Controllers
{
    public static class AccountController
    {
		public static void Update(this Account oldInformation, Account newInfromation)
		{
			foreach (PropertyInfo property in newInfromation.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldInformation.GetType().GetProperty(property.Name).SetValue(oldInformation, newInfromation.GetType().GetProperty(property.Name).GetValue(newInfromation));
			}
		}
	}
}
