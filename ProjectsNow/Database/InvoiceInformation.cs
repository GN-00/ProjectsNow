using System;

namespace ProjectsNow.Database
{
    public class InvoiceInformation
    {
        public long CompanyVAT { get { return Database.DatabaseAI.CompanyVAT; } }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string JobOrderCode { get; set; }
        public string POs { get; set; } = "";
        public string CustomerName { get; set; }
        public string Attention { get; set; }
        public string ProjectName { get; set; }
        public string Address { get; set; }
        public string CustomerVAT { get; set; }

    }
}
