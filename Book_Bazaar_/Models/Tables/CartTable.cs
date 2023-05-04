namespace Book_Bazaar_.Models
{
    public class CartTable
    {
        public Guid BookID { get; set; }
        public Guid UserID { get; set; }
        public int Quantity { get; set; }
    }
}
