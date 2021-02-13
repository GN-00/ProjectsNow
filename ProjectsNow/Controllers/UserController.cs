using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class UserController
    {
        public static void Update(this User oldUser, User newUser)
        {
            foreach (PropertyInfo property in newUser.GetType().GetProperties())
            {
                if (property.SetMethod != null)
                    oldUser.GetType().GetProperty(property.Name).SetValue(oldUser, newUser.GetType().GetProperty(property.Name).GetValue(newUser));
            }
        }

        public static User GetUser(SqlConnection connection, string userName, string password)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where UserName = '{userName}' And Password = '{password}'");
            return record;
        }

        public static User CheckUserInquiryID(SqlConnection connection, int inquiryID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where InquiryID = {inquiryID}");
            return record;
        }
        public static User CheckUserQuotationID(SqlConnection connection, int quotationID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where QuotationID = {quotationID}");
            return record;
        }
        public static User CheckUserJobOrderID(SqlConnection connection, int jobOrderID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where JobOrderID = {jobOrderID}");
            return record;
        }
        public static User CheckUserCustomerID(SqlConnection connection, int customerID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where CustomerID = {customerID}");
            return record;
        }
        public static User CheckUserContactID(SqlConnection connection, int contactID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where ContactID = {contactID}");
            return record;
        }
        public static User CheckUserConsultantID(SqlConnection connection, int consultantID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where ConsultantID = {consultantID}");
            return record;
        }
        public static User CheckUserSupplierID(SqlConnection connection, int supplierID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where SupplierID = {supplierID}");
            return record;
        }
        public static User CheckUserAcknowledgementID(SqlConnection connection, int acknowledgementID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where AcknowledgementID = {acknowledgementID}");
            return record;
        }
        public static User CheckUserAccountID(SqlConnection connection, int accountID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where AccountID = {accountID}");
            return record;
        }
        public static User CheckUserJobOrderFinanceID(SqlConnection connection, int jobOrderFinanceID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where JobOrderFinanceID = {jobOrderFinanceID}");
            return record;
        }



        public static void UpdateInquiryID(SqlConnection connection, User user)
        {
            if(user.InquiryID == null)
                connection.Execute($"Update [User].[Users] Set InquiryID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set InquiryID = {user.InquiryID} Where UserID = {user.UserID}");
        }
        public static void UpdateQuotationID(SqlConnection connection, User user)
        {
            if (user.QuotationID == null)
                connection.Execute($"Update [User].[Users] Set QuotationID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set QuotationID = {user.QuotationID} Where UserID = {user.UserID}");
        }
        public static void UpdateJobOrderID(SqlConnection connection, User user)
        {
            if (user.JobOrderID == null)
                connection.Execute($"Update [User].[Users] Set JobOrderID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set JobOrderID = {user.JobOrderID} Where UserID = {user.UserID}");
        }
        public static void UpdateCustomerID(SqlConnection connection, User user)
        {
            if (user.CustomerID == null)
                connection.Execute($"Update [User].[Users] Set CustomerID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set CustomerID = {user.CustomerID} Where UserID = {user.UserID}");
        }
        public static void UpdateContactID(SqlConnection connection, User user)
        {
            if (user.ContactID == null)
                connection.Execute($"Update [User].[Users] Set ContactID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set ContactID = {user.ContactID} Where UserID = {user.UserID}");
        }
        public static void UpdateConsultantID(SqlConnection connection, User user)
        {
            if (user.ConsultantID == null)
                connection.Execute($"Update [User].[Users] Set ConsultantID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set ConsultantID = {user.ConsultantID} Where UserID = {user.UserID}");
        }
        public static void UpdateSupplierID(SqlConnection connection, User user)
        {
            if (user.SupplierID == null)
                connection.Execute($"Update [User].[Users] Set SupplierID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set SupplierID = {user.SupplierID} Where UserID = {user.UserID}");
        }
        public static void UpdateAcknowledgementID(SqlConnection connection, User user)
        {
            if (user.SupplierID == null)
                connection.Execute($"Update [User].[Users] Set AcknowledgementID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set AcknowledgementID = {user.AcknowledgementID} Where UserID = {user.UserID}");
        }
        public static void UpdateAccountID(SqlConnection connection, User user)
        {
            if (user.SupplierID == null)
                connection.Execute($"Update [User].[Users] Set AccountID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set AccountID = {user.AccountID} Where UserID = {user.UserID}");
        }
        public static void UpdateJobOrderFinanceID(SqlConnection connection, User user)
        {
            if (user.SupplierID == null)
                connection.Execute($"Update [User].[Users] Set JobOrderFinanceID = NULL Where UserID = {user.UserID}");
            else
                connection.Execute($"Update [User].[Users] Set JobOrderFinanceID = {user.JobOrderFinanceID} Where UserID = {user.UserID}");
        }

        public static void ResetIDs(SqlConnection connection, int userID)
        {
            connection.Execute($"Update [User].[Users] Set " +
                               $"QuotationID = NULL, " +
                               $"InquiryID = NULL, " +
                               $"JobOrderID = NULL, " +
                               $"ConsultantID = NULL, " +
                               $"ContactID = NULL, " +
                               $"CustomerID = NULL, " +
                               $"SupplierID = NULL, " +
                               $"AcknowledgementID = NULL, " +
                               $"AccountID = NULL, " +
                               $"JobOrderFinanceID = NULL " +
                               $"Where UserID = {userID}");
        }
    }
}
