namespace Book_Bazaar_.Models
{
    public class Orders
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string OrderNo { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
    }
}
