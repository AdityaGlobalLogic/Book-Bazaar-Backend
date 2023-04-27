namespace Book_Bazaar_.Models
{
    public class Books
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ISBN { get; set; } 
        public string BookImage { get; set; }
        public int status { get; set; }
    }
}
