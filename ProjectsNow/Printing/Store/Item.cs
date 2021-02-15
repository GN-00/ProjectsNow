namespace ProjectsNow.Printing.Store
{
	public class Item 
	{
        public int ID { get; set; }
        public int InvoiceID { get; set; }
		public int SN { get; set; }
        public string Supplier { get; set; }
		public string InvoiceNumber { get; set; }
		public string Category { get; set; }
		public string Code { get; set; }
		public string PartNumber
		{ get { return ($"{Category}{Code}"); } }
		public string Description { get; set; }
		public double Qty { get; set; }
		public double Cost { get; set; }
		public double TotalCost { get { return (Qty * Cost); } }
		public double VAT { get; set; }
		public double VATValue { get { return (VAT / 100) * TotalCost; } }
		public double TotalPrice { get { return (Qty * Cost * (1 + VAT / 100)); } }
	}
}
