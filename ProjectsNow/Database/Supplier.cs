using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
    [ReadTable("[Store].[Suppliers]")]
    [WriteTable("[Store].[Suppliers]")]
    public class Supplier
    {
        [ID]public int ID { get; set; }
        public int BankID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
