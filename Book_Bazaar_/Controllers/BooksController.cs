using Book_Bazaar_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        public BooksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("api/books")]
        public async  Task<ActionResult> GetBooks()
        {
            List<Books> books = new List<Books>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Retrieve list of books from database
                using (SqlCommand command = new SqlCommand("SELECT * FROM Books", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Books book = new Books
                            {
                                BookID = (int)reader["BookID"],
                                Title = (string)reader["Title"],
                                Description = (string)reader["Description"],
                                AuthorName = (string)reader["AuthorName"],
                                Price = (decimal)reader["Price"],
                                Quantity= (int)reader["Quantity"],
                                ISBN = (int)reader["ISBN"],
                                BookImage = (string)reader["BookImage"],
                                UserID = (int)reader["UserID"],
                                CategoryID = (int)reader["CategoryID"]
                            };

                            books.Add(book);
                        }
                    }
                }
            }

            return Ok(books);
        }
    }
}
}
