namespace Book_Bazaar_.Models
{
    public class Orders
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PinCode { get; set; }
    }
}
