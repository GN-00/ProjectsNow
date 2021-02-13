using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class CustomerController
    {
        public static void Update(this Customer oldCustomer, Customer newCustomer)
        {
            foreach (PropertyInfo property in newCustomer.GetType().GetProperties())
            {
                if (property.SetMethod != null)
                    oldCustomer.GetType().GetProperty(property.Name).SetValue(oldCustomer, newCustomer.GetType().GetProperty(property.Name).GetValue(newCustomer));
            }
        }

        public static ObservableCollection<Customer> GetCustomers(SqlConnection connection)
        {
            var query = $"Select CustomerID, Customers.SalesmanID, CustomerName, City, Address, Phone, Email, Website, StartRelation, VATNumber, Note, POBox, " +
                        $"EmployeeName as SalesmanName " +
                        $"From [Customer].[Customers]" +
                        $"LEFT OUTER JOIN [User].[Employees] On Employees.EmployeeID = Customers.SalesmanID " +
                        $"Order By CustomerName";
            var customers = new ObservableCollection<Customer>(connection.Query<Customer>(query));
            return customers;
        }

        public static bool AbilityToDelete(this Customer customer, SqlConnection connection)
        {
            var query = $"Select CustomerID " +
                        $"From [Inquiry].[Inquiries] " +
                        $"Where CustomerID = {customer.CustomerID} ";
            var customers = connection.Query<Customer>(query).ToList();
            return (customers.Count == 0 ? true : false);
        }
    }
}
