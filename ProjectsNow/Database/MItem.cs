using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[JobOrder].[PanelsItems]")]
    [WriteTable("[JobOrder].[PanelsItems]")]
    public class MItem : INotifyPropertyChanged  //Modification Item
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ItemID { get; set; }
		public int PanelID { get; set; }
		public int PanelSN { get; set; }
		public string PanelName { get; set; }
		public int PanelQty { get; set; }

		private string _Category;
		public string Category
		{
			get { return this._Category; }
			set { if (value != this._Category) { this._Category = value; NotifyPropertyChanged(); NotifyPropertyChanged("PartNumber"); } }
		}

		private string _Code;
		public string Code
		{
			get { return this._Code; }
			set { if (value != this._Code) { this._Code = value; NotifyPropertyChanged(); NotifyPropertyChanged("PartNumber"); } }
		}

		[DontRead]
		[DontWrite]
		public string PartNumber
		{ get { return ($"{Category}{Code}"); } }

		private string _Description;
		public string Description
		{
			get { return this._Description; }
			set { if (value != this._Description) { this._Description = value; NotifyPropertyChanged(); } }
		}

		public string Unit { get; set; }

		private double _ItemQty;
		public double ItemQty
		{
			get { return this._ItemQty; }
			set { if (value != this._ItemQty) { this._ItemQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("TotalQty"); } }
		}

		public double TotalQty
		{ get { return (this.PanelQty * this.ItemQty); } }

		private string _Brand;
		public string Brand
		{
			get { return this._Brand; }
			set { if (value != this._Brand) { this._Brand = value; NotifyPropertyChanged(); } }
		}

		public double ItemCost { get; set; }
		public double ItemDiscount { get; set; }

		[DontRead][DontWrite]public double ItemPrice
		{
			get { return Math.Round(this.ItemCost * (1 - (this.ItemDiscount / 100)), 3); }
		}
		[DontRead][DontWrite]public double ItemTotalCost
		{
			get { return Math.Round((this.ItemCost * this.ItemQty), 3); }
		}
		[DontRead][DontWrite]public double ItemTotalPrice
		{
			get { return Math.Round((this.ItemCost * this.ItemQty * (1 - (this.ItemDiscount / 100))), 3); }
		}

		public string ItemTable { get; set; }

		private string _ItemType;
		public string ItemType
		{
			get { return this._ItemType; }
			set { if (value != this._ItemType) { this._ItemType = value; NotifyPropertyChanged(); } }
		}
		public int ItemSort { get; set; }
		public string Source { get; set; }
		public int ModificationID { get; set; }
		public DateTime Date { get; set; }

	}
}
