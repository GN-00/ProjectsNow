using Dapper;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class QuotationOptionController
    {
        public static void Update(this QuotationOption quotationOption, QuotationOption newQuotationOption)
        {
            foreach (PropertyInfo property in newQuotationOption.GetType().GetProperties())
            {
                if (property.SetMethod != null)
                    quotationOption.GetType().GetProperty(property.Name).SetValue(quotationOption, newQuotationOption.GetType().GetProperty(property.Name).GetValue(newQuotationOption));
            }
        }

        public static ObservableCollection<QuotationOption> GetOptions(SqlConnection connection, int quotationID)
        {
            var query = $"{DatabaseAI.GetFields<QuotationOption>()}" +
                        $"WHERE QuotationsOptions.QuotationID = {quotationID} ";

            var records = new ObservableCollection<QuotationOption>(connection.Query<QuotationOption>(query));
            return records;
        }
    }
}
