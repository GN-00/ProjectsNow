using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[Quotation].[QuotationsPanelsItems]")]
	[WriteTable("[Quotation].[QuotationsPanelsItems]")]
	public class QItem : INotifyPropertyChanged //Quotation Item
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ItemID { get; set; }
		public int PanelID { get; set; }

		private string _Article1;
		public string Article1
		{
			get { return this._Article1; }
			set { if (value != this._Article1) { this._Article1 = value; NotifyPropertyChanged(); } }
		}

		private string _Article2;
		public string Article2
		{
			get { return this._Article2; }
			set { if (value != this._Article2) { this._Article2 = value; NotifyPropertyChanged(); } }
		}

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

		public string Unit { get; set; } = "No";

		private double _ItemQty;
		public double ItemQty
		{
			get { return this._ItemQty; }
			set
			{
				if (value != this._ItemQty)
				{
					this._ItemQty = value;

					NotifyPropertyChanged("ItemTotalCost");
					NotifyPropertyChanged("ItemTotalPrice");
					NotifyPropertyChanged();
				}
			}
		}

		private string _Brand;
		public string Brand
		{
			get { return this._Brand; }
			set { if (value != this._Brand) { this._Brand = value; NotifyPropertyChanged(); } }
		}

		private string _Remarks;
		public string Remarks
		{
			get { return this._Remarks; }
			set { if (value != this._Remarks) { this._Remarks = value; NotifyPropertyChanged(); } }
		}

		private double _ItemCost;
		public double ItemCost
		{
			get { return this._ItemCost; }
			set
			{
				if (value != this._ItemCost)
				{
					if (value < 0)
						this._ItemCost = 0;
					else
						this._ItemCost = value;

					NotifyPropertyChanged("ItemPrice");
					NotifyPropertyChanged("ItemTotalCost");
					NotifyPropertyChanged("ItemTotalPrice");
					NotifyPropertyChanged();
				}
			}
		}

		private double _ItemDiscount;
		public double ItemDiscount
		{
			get { return this._ItemDiscount; }
			set
			{
				if (value != this._ItemDiscount)
				{
					if (value > 100)
						this._ItemDiscount = 0;
					else
						this._ItemDiscount = value;

					NotifyPropertyChanged();
				}
			}
		}

		[DontRead]
		[DontWrite]
		public double ItemPrice
		{
			get { return Math.Round(this._ItemCost * (1 - (this._ItemDiscount / 100)), 3); }
		}

		[DontRead]
		[DontWrite]
		public double ItemTotalCost
		{
			get { return Math.Round((this._ItemCost * this._ItemQty), 3); }
		}

		[DontRead]
		[DontWrite]
		public double ItemTotalPrice
		{
			get { return Math.Round((this._ItemCost * this._ItemQty * (1 - (this._ItemDiscount / 100))), 3); }
		}

		public string ItemTable { get; set; }

		private string _ItemType;
		public string ItemType
		{
			get { return this._ItemType; }
			set { if (value != this._ItemType) { this._ItemType = value; NotifyPropertyChanged(); } }
		}

		public int ItemSort { get; set; }
		public string SelectionGroup { get; set; }


		//RecalculateItems Only
		[DontWrite] public double ReferenceCost { get; set; }
		[DontWrite] public double ReferenceDiscount { get; set; }

	}
}
