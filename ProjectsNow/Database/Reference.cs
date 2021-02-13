using ProjectsNow.Enums;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;
using System;

namespace ProjectsNow.Database
{
    [ReadTable("[Reference].[References]")]
	[WriteTable("[Reference].[References]")]
	public class Reference : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ReferenceID { get; set; }

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

		public string Article1 { get; set; }
		public string Article2 { get; set; }

		private string _Brand;
		public string Brand
		{
			get { return this._Brand; }
			set { if (value != this._Brand) { this._Brand = value; NotifyPropertyChanged(); } }
		}
		public string Remarks { get; set; }

		private string _Unit;
		public string Unit
		{
			get { return this._Unit; }
			set { if (value != this._Unit) { this._Unit = value; NotifyPropertyChanged(); } }
		}
		//[DontRead] [DontWrite] public double ItemQty { get; set; } = 1;

		private double _Cost;
		public double Cost
		{
			get { return this._Cost; }
			set { if (value != this._Cost) { this._Cost = value; NotifyPropertyChanged(); } }
		}

		private double _Discount;
		public double Discount
		{
			get { return this._Discount; }
			set { if (value != this._Discount) { this._Discount = value; NotifyPropertyChanged(); } }
		}
		//[DontRead] [DontWrite] public string ItemTable { get; set; }
		[DontRead] [DontWrite] public string ItemType { get; set; } = ItemTypes.Standard.ToString();
		public string Type { get; set; }

		//public Reference(QItem item)
		//{
		//	this.Category = item.Category;
		//	this.Code = item.Code;
		//	this.Description = item.Description;
		//	this.Article1 = item.Article1;
		//	this.Article2 = item.Article2;
		//	this.Brand = item.Brand;
		//	this.ItemTable = item.ItemTable;
		//	this.Remarks = item.Remarks;
		//	this.ItemQty = item.ItemQty;
		//	this.Cost = item.ItemCost;
		//	this.Discount = item.ItemDiscount;
		//	this.Unit = item.Unit;
		//	this.ItemType = item.ItemType;
		//}
	}
}
