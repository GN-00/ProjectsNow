using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
    [ReadTable("[Quotation].[QuotationsOptionsPanels]")]
    [WriteTable("[Quotation].[QuotationsOptionsPanels]")]
    public class QuotationOptionPanel
    {
        [ID] public int ID { get; set; }
        public int OptionID { get; set; }
        [JoinID("[Quotation].[QuotationsPanels]")] public int PanelID { get; set; }
        [DontWrite] [Join("[Quotation].[QuotationsPanels]")] public int PanelSN { get; set; }
        [DontWrite] [Join("[Quotation].[QuotationsPanels]")] public string PanelName { get; set; }
        [DontWrite] [Join("[Quotation].[QuotationsPanels]")] public int PanelQty { get; set; }
        [DontWrite] [Join("[Quotation].[QuotationsPanels]")] public string EnclosureName { get; set; }
    }
}
