namespace Book_Bazaar_.Models
{
    public class OrderItems
    {
        public int OrderItemsID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public decimal price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
