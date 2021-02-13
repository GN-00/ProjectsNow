using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
    [ReadTable("QuotationsItems")]
    public class DiscountSheetItem
    {
        public int QuotationID { get; set; }

        public string Category { get; set; }
        public string Code { get; set; }
        [DontRead]
        [DontWrite]
        public string PartNumber
        { get { return ($"{Category}{Code}"); } }
        public string Description { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public double Discount { get; set; }
        public string Brand { get; set; }
        public string Article1 { get; set; }
        public string Article2 { get; set; }
    }
}
