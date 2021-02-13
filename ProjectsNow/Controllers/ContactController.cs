using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class ContactController
    {
		public static void Update(this Contact oldContact, Contact newContact)
		{
			foreach (PropertyInfo property in newContact.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					oldContact.GetType().GetProperty(property.Name).SetValue(oldContact, newContact.GetType().GetProperty(property.Name).GetValue(newContact));
			}
		}

		public static ObservableCollection<Contact> GetContacts(SqlConnection connection)
		{
			var query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
						$"CustomerName " +
						$"From [Customer].[Contacts]" +
						$"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
						$"Order By ContactName";
			var records = new ObservableCollection<Contact>(connection.Query<Contact>(query));
			return records;
		}
		public static ObservableCollection<Contact> GetCustomerContacts(SqlConnection connection, int customerID)
		{
			var query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
						$"CustomerName " +
						$"From [Customer].[Contacts]" +
						$"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
						$"Where Contacts.CustomerID = {customerID} " +
						$"Order By ContactName";
			var records = new ObservableCollection<Contact>(connection.Query<Contact>(query));
			return records;
		}
		public static ObservableCollection<Contact> GetCustomerRemainingContacts(SqlConnection connection, int customerID, int[] contactsIDs)
		{
			var query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
						$"CustomerName " +
						$"From [Customer].[Contacts] " +
						$"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
						$"Where Contacts.CustomerID = {customerID} ";


			if (contactsIDs.Count() != 0)
			{
				query += $"And ContactID Not in ";
				for (int i = 0; i < contactsIDs.Count(); i++)
				{
					query += (i == 0 ? "(" : ", ") + contactsIDs[i];
				}
				query += $") Order By ContactName";
			}
			else
			{
				query += $"Order By ContactName";
			}

			var records = new ObservableCollection<Contact>(connection.Query<Contact>(query));
			return records;
		}
		public static ObservableCollection<Contact> GetProjectContacts(SqlConnection connection, int inquiryID)
		{
			var query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
						$"ProjectsContacts.Attention " +
						$"FROM [Inquiry].[ProjectsContacts] " +
						$"LEFT OUTER JOIN [Customer].[Contacts] On Contacts.ContactID = ProjectsContacts.ContactID " +
						$"Where InquiryID = {inquiryID} " +
						$"Order By ContactName ";
			var records = new ObservableCollection<Contact>(connection.Query<Contact>(query));
			return records;
		}

		public static Contact GetProjectAttention(SqlConnection connection, int inquiryID)
		{
			var query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
						$"Attention " +
						$"FROM [Inquiry].[ProjectsContacts] " +
						$"LEFT OUTER JOIN [Customer].[Contacts] On Contacts.ContactID = ProjectsContacts.ContactID " +
						$"Where InquiryID = {inquiryID} And Attention = 'True'";

			var record = connection.Query<Contact>(query).FirstOrDefault();

			return record;
		}

		public static bool AbilityToDelete(this Contact contact, SqlConnection connection)
		{
			var query = $"Select ContactID " +
						$"From [Inquiry].[ProjectsContacts] " +
						$"Where ContactID = {contact.ContactID} ";
			var records = connection.Query<Contact>(query).ToList();
			return (records.Count == 0 ? true : false);
		}
	}
}
