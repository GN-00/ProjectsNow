using System;

namespace ProjectsNow.Database
{
    public class IPanel
    {
        public int PanelID { get; set; }
        public string PurchaseOrdersNumber { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public int InvoicedQty { get; set; }
        public string PanelType { get; set; }
        public string PanelTypeArbic 
        {
            get
            {
                if (PanelType == "Power Panel")
                    return "لوحة توزيع كهربائية";
                else if(PanelType == "Control Panel")
                    return "لوحة تحكم كهربائية";
                else if (PanelType == "Ready Made Panel")
                    return "لوحة توزيع كهربائية";
                else if (PanelType == "ECB")
                    return "لوحة توزيع كهربائية";
                else if (PanelType == "ATS")
                    return "لوحة تحويل كهربائية أوتوماتيكية";
                else if (PanelType == "MTS")
                    return "لوحة تحويل كهربية يدوية";
                else if (PanelType == "MCC")
                    return "لوحة تحكم كهربائية";
                else if (PanelType == "PFC")
                    return "لوحة تصحيح معامل القدرة";
                else if (PanelType == "Synchronizing Panel")
                    return "لوحة مزامنة مولدات كهربائية";
                else if (PanelType == "Fuse Panel")
                    return "لوحة توزيع كهربائية";
                else if (PanelType == "Junction Box")
                    return "لوحة توزيع كهربائية";
                else
                    return null;
            }
        }
        public double VAT { get; set; } //0.15
        public double Discount { get; set; } //0.15
        public double PanelProfit { get; set; }
        public double PanelEstimatedCost { get; set; }
        public double PanelEstimatedPrice
        {
            get { return Math.Round((this.PanelEstimatedCost * (1 - this.Discount / 100) / (1 - this.PanelProfit / 100)), 3); }
        }
        public double PanelsEstimatedPrice
        {
            get { return Math.Round((this.PanelEstimatedCost * (1 - this.Discount / 100) * this.InvoicedQty / (1 - this.PanelProfit / 100)), 3); }
        }
        public double VATValue
        {
            get { return Math.Round(this.PanelsEstimatedPrice * VAT, 3); }
        }
        public double FinalPrice
        {
            get { return Math.Round((PanelsEstimatedPrice * (1 + VAT)), 3); }
        }

    }
}
