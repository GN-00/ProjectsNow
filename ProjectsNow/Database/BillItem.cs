using System;
using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
	[ReadTable("PanelsItems")]
	[WriteTable("PanelsItems")]
	public class BillItem
	{
		[ID] public int ItemID { get; set; }
		public int PanelID { get; set; }
		public string Article1 { get; set; }
		public string Article2 { get; set; }
		public string Category { get; set; }
		public string Code { get; set; }
		[DontRead]
		[DontWrite]
		public string PartNumber
		{ get { return ($"{Category}{Code}"); } }
		public string Description { get; set; }
		public string Unit { get; set; }
		public double ItemQty { get; set; }
		public string Brand { get; set; }
		public string Remarks { get; set; }
		public double ItemCost { get; set; }
		public double ItemDiscount { get; set; }

		[DontRead]
		[DontWrite]
		public double ItemPrice
		{
			get { return Math.Round(this.ItemCost * (1 - (this.ItemDiscount / 100)), 3); }
		}

		[DontRead]
		[DontWrite]
		public double ItemTotalCost
		{
			get { return Math.Round((this.ItemCost * this.ItemQty), 3); }
		}

		[DontRead]
		[DontWrite]
		public double ItemTotalPrice
		{
			get { return Math.Round((this.ItemCost * this.ItemQty * (1 - (this.ItemDiscount / 100))), 3); }
		}

		public string ItemTable { get; set; }
		public string ItemType { get; set; }
		public int ItemSort { get; set; }

	}
}
