
using Book_Bazaar_.Models;
using Book_Bazaar_.Models.AWS;
using Book_Bazaar_.Models.Tables;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using static System.Reflection.Metadata.BlobBuilder;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;
        public static IDictionary<Guid, string> url = new Dictionary<Guid, string>();

        public VendorController(IConfiguration configuration, IStorageService storageService)
        {
            _configuration = configuration;
            _storageService = storageService;
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file, Guid userId)
        {
            //Process the file
            await using var memoryStr = new MemoryStream();
            await file.CopyToAsync(memoryStr);

            var fileExt = Path.GetExtension(file.FileName);
            var objName = $"{Guid.NewGuid()}.{fileExt}";

            var s3Obj = new S3Object()
            {
                BucketName = "bookbazaar",
                InputStream = memoryStr,
                Name = objName
            };

            var cred = new AwsCredentials()
            {
                AwsKey = _configuration["AwsConfiguration:AWSAccessKey"],
                AwsSecretKey = _configuration["AwsConfiguration:AWSSecretKey"]
            };

            var result = await _storageService.UploadFileAsync(s3Obj, cred);

            //get url request
            var Imageurl = $"https://bookbazaar.s3.amazonaws.com/{objName}";
            url.Add(userId, Imageurl);

            return Ok(new 
            { 
                result,
                url
            });
        }

        [HttpPost]
        [Route("{userId}/ConvertToVendor")]
        public async Task<ActionResult> ConvertToVendor(Guid userId)
        {
            // Connect to the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                // Create a SQL command to retrieve the user by ID
                string query = "SELECT * FROM Users WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@UserID", userId);

                // Open the connection and execute the command
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return NotFound();
                }

                reader.Close();

                // Create a SQL command to update the user's IsVendor property
                string updateQuery = "UPDATE Users SET IsVendor = @IsVendor WHERE UserID = @UserID";
                SqlCommand updateCommand = new SqlCommand(updateQuery, conn);
                updateCommand.Parameters.AddWithValue("@IsVendor", true);
                updateCommand.Parameters.AddWithValue("@UserID", userId);

                // Execute the update command
                updateCommand.ExecuteNonQuery();

                return Ok(new
                {
                    message = "User is now Vendor"
                });
            }
        }

        [HttpGet]
        [Route("{userId}/GetVendor_Published_Books")]
        public async Task<ActionResult> GetVendor_Published_Books(Guid userId)
        {
            List<Books> books = new List<Books>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Books where UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID",userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Books book = new Books
                            {
                                BookID = (Guid)reader["BookID"],
                                Title = (string)reader["Title"],
                                Description = (string)reader["Description"],
                                AuthorName = (string)reader["AuthorName"],
                                Price = (decimal)reader["Price"],
                                Quantity = (int)reader["Quantity"],
                                ISBN = (string)reader["ISBN"],
                                BookImage = (string)reader["BookImage"],
                                UserID = (Guid)reader["UserID"],
                                CategoryID = (Guid)reader["CategoryID"],
                                Rating = (decimal)reader["Rating"],
                            };
                            books.Add(book);   
                        }
                    }
                }
                connection.Close();
            }
            return Ok(books);
        }

        [HttpPost]
        [Route("{userId}/PublishBook")]
        public async Task<ActionResult> PublishBook(Guid userId,[FromBody] BookModel book)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                conn.Open();
                // Create a SQL command to retrieve the user by ID
                using (SqlCommand command =new SqlCommand ("SELECT * FROM Users WHERE UserID = @UserID",conn))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var check = (bool)reader["IsVendor"];
                            if(check)
                            {
                                reader.Close();
 
                                using (SqlCommand insertcommand = new SqlCommand("INSERT INTO Books (Title, Description, AuthorName, Price, Quantity, ISBN, BookImage, UserID, CategoryID,Rating) VALUES ( @Title, @Description, @AuthorName, @Price, @Quantity, @ISBN, @BookImage,@UserID, @CategoryID,@Rating)", conn))
                                {
                                    // Add parameters to the command

                                    insertcommand.Parameters.AddWithValue("@Title", book.Title);
                                    insertcommand.Parameters.AddWithValue("@Description", book.Description);
                                    insertcommand.Parameters.AddWithValue("@AuthorName", book.AuthorName);
                                    insertcommand.Parameters.AddWithValue("@Price", book.Price);
                                    insertcommand.Parameters.AddWithValue("@Quantity", book.Quantity);
                                    insertcommand.Parameters.AddWithValue("@ISBN", book.ISBN);
                                    insertcommand.Parameters.AddWithValue("@BookImage", url[userId]);
                                    insertcommand.Parameters.AddWithValue("@UserID", userId);
                                    insertcommand.Parameters.AddWithValue("@CategoryID", book.CategoryID);
                                    insertcommand.Parameters.AddWithValue("@Rating", book.Rating);

                                    // Open connection and execute comman
                                    insertcommand.ExecuteNonQuery();
                                    conn.Close();
                                }
                                url.Remove(userId);
                                return Ok(new { message = "Book published successfully" });

                            }
                            else
                            {
                                reader.Close();
                                return BadRequest(new { message = "User not allowed to publish book" });
                            }

                        }
                        else
                        {
                            return BadRequest(new { message = "User does not exist" });
                        }

                    }

                }
               
            }
            
        }

        [HttpPost]
        [Route("{userId}/{bookId}/delete-book")]
        public async Task<ActionResult> DeleteBook(Guid bookId)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                conn.Open();
                using (SqlCommand deleteCommand = new SqlCommand("delete from Books WHERE BookID = @BookID", conn))
                {

                    deleteCommand.Parameters.AddWithValue("@BookID", bookId);
                    deleteCommand.ExecuteNonQuery();
                }
                conn.Close();
                return Ok(new
                {
                    message = "Book removed from inventory."
                });
            }
        }

    }
}
