namespace Book_Bazaar_.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public decimal price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
