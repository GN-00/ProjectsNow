using System;
using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
	[ReadTable("QuotationsPanels")]
	[WriteTable("QuotationsPanels")]
	public class BillPanel
	{
		[ID] public int PanelID { get; set; }
		public int QuotationID { get; set; }
		public int? PanelSN { get; set; }
		public string PanelName { get; set; }
		public int PanelQty { get; set; }
		public double PanelDiscount { get; set; }
		public double PanelProfit { get; set; }
		[DontWrite] public double PanelCost { get; set; }

		[DontRead]
		[DontWrite]
		public double PanelPrice
		{
			get { return Math.Round((this.PanelCost / (1 - this.PanelProfit / 100)), 3); }
		}

		[DontRead]
		[DontWrite]
		public double PanelsPrice
		{
			get { return Math.Round((this.PanelCost * this.PanelQty / (1 - this.PanelProfit / 100)), 3); }
		}

		//Technical
		public string EnclosureType { get; set; }
		public string EnclosureInstallation { get; set; }
		public string EnclosureLocation { get; set; }

		public double? EnclosureHeight { get; set; }
		public double? EnclosureWidth { get; set; }
		public double? EnclosureDepth { get; set; }
		public string EnclosureIP { get; set; }

		public string Source { get; set; }
		public string Frequency { get; set; }
		public string EarthingSystem { get; set; }

	}
}
