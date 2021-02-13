using Dapper;
using System.Linq;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    public static class EstimationController
    {
        public static List<Estimation> GetEstimation(SqlConnection connection)
        {
            var query = $"Select EmployeeID as EstimationID, EmployeeName as EstimationName " +
                        $"From [User].[Employees]" +
                        $"Where EmployeeJob Like '%Estimation%' " +
                        $"Order By EstimationName";
            var estimations = connection.Query<Estimation>(query).ToList();
            estimations.Add(new Estimation());
            return estimations;
        }
    }
}
