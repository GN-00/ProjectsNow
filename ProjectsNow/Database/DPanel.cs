
namespace ProjectsNow.Database
{
    public class DPanel
    {
        public string DeliveryNumber { get; set; }
        public int PanelID { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public int PanelQty { get; set; }
        public int PreviousQty { get; set; }
        public int DeliveredQty { get; set; }
        public int Outstanding 
        {
            get { return (PanelQty - DeliveredQty - PreviousQty); }
        }


    }
}
