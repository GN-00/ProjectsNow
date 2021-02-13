using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
    [ReadTable("[User].[Users]")]
    [WriteTable("[User].[Users]")]
    public class User
    {
        [ID] public int UserID { get; set; } /* is same EmploeeID */
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserCode { get; set; }
        public string UserFullName { get; set; }
        public bool Administrator { get; set; }

        public int? InquiryID { get; set; }
        public int? QuotationID { get; set; }
        public int? JobOrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? ContactID { get; set; }
        public int? ConsultantID { get; set; }
        public int? SupplierID { get; set; }
        public int? AcknowledgementID { get; set; }
        public int? AccountID { get; set; }
        public int? JobOrderFinanceID { get; set; }

        #region Tendaring
        public bool QuotationsDiscount { get; set; }
        public double QuotationsDiscountValue { get; set; }
        public bool AccessTendaring { get; set; }
        public bool AccessInquiries { get; set; }
            public bool AccessInquiriesData { get; set; }
            public bool InquiriesAdd { get; set; }
            public bool InquiriesEdit { get; set; }
            public bool InquiriesDelete { get; set; }
            public bool InquiriesReassign { get; set; }

        public bool AccessQuote { get; set; }
            public bool QuoteManager { get; set; }
            public bool QuotationItemsDiscount { get; set; }
        public bool AccessQuotations { get; set; }
            public bool QuotationsManage { get; set; }


        public bool AccessCustomers { get; set; }
        //public bool AccessCustomersData { get; set; } in case need it
        public bool CustomersWindowAdd { get; set; }
        public bool CustomersWindowEdit { get; set; }
        public bool CustomersWindowDelete { get; set; }
        public bool CustomersWindowHistory { get; set; }

        public bool AccessConsultants { get; set; }
        public bool ConsultantsWindowAdd { get; set; }
        public bool ConsultantsWindowEdit { get; set; }
        public bool ConsultantsWindowDelete { get; set; }

        public bool AccessContacts { get; set; }
        public bool ContactsWindowAdd { get; set; }
        public bool ContactsWindowEdit { get; set; }
        public bool ContactsWindowDelete { get; set; }

        #endregion 

        #region Projects
        public bool AccessProjects { get; set; }
            public bool AccessNewJobOrder { get; set; }
            public bool AccessJobOrders { get; set; }
                public bool JobOrderInformation { get; set; }
                public bool JobOrderAcknowledgement { get; set; }
                public bool JobOrderPurchaseOrders { get; set; }
                public bool JobOrderPanels { get; set; }
                public bool JobOrderPosting { get; set; }
                public bool JobOrderInvoicing { get; set; }


        #endregion

        #region Items
        public bool AccessItems { get; set; }
        public bool AccessReferences { get; set; }
            public bool ReferencesDiscount { get; set; }
        public bool AccessStore { get; set; }
        #endregion

        #region Finance
        public bool AccessFinance { get; set; }
        public bool AccessCompanyAccount { get; set; }
        public bool AccessJobordersFinance { get; set; }
        #endregion

    }
}