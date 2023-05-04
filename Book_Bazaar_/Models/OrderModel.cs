using Book_Bazaar_.Models.Tables;

namespace Book_Bazaar_.Models
{
    public class OrderModel
    {
        public Guid UserID { get; set; }
        public decimal OrderTotal { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Pincode { get; set; }
    }
}
