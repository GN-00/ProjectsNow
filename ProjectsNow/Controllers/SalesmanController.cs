using Dapper;
using System.Linq;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    public static class SalesmanController
    {
        public static List<Salesman> GetSalesmen(SqlConnection connection)
        {
            var query = $"Select EmployeeID as SalesmanID, EmployeeName as SalesmanName " +
                        $"From [User].[Employees]" +
                        $"Where EmployeeJob Like '%Salesman%'" +
                        $"Order By SalesmanName";
            var salesmen = connection.Query<Salesman>(query).ToList();
            salesmen.Add(new Salesman());
            return salesmen;
        }

        public static Salesman GetSalesman(SqlConnection connection, int salesmanID)
        {
            var query = $"Select EmployeeID as SalesmanID, EmployeeName as SalesmanName " +
                        $"From [User].[Employees]" +
                        $"Where EmployeeJob Like '%Salesman%' And EmployeeID = {salesmanID}" +
                        $"Order By SalesmanName";
            var salesmen = connection.QueryFirstOrDefault<Salesman>(query);
            return salesmen;
        }
    }
}
