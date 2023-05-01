using System.ComponentModel.DataAnnotations;

namespace Book_Bazaar_.Models.Tables
{
    public class Books
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ISBN { get; set; }
        public string BookImage { get; set; }
        public int UserID { get; set; }
        public int? CategoryID { get; set; }
        [Range(0,5,ErrorMessage = "Enter value greater than 0 and less than 5")]
        public decimal Rating { get; set; }
    }
}
