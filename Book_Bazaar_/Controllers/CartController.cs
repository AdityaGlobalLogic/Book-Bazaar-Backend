using Book_Bazaar_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/cart/add")]
        public async Task<ActionResult> AddToCart(Cart item)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Check if item already exists in cart
                using (SqlCommand command = new SqlCommand("SELECT * FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", item.UserID);
                    command.Parameters.AddWithValue("@BookID", item.BookID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Item already exists, update quantity
                            int quantity = (int)reader["Quantity"] + 1;
                            reader.Close();
                            using (SqlCommand updateCommand = new SqlCommand("UPDATE Cart SET Quantity = @Quantity WHERE UserID = @UserID AND BookID = @BookID", connection))
                            {
                                updateCommand.Parameters.AddWithValue("@UserID", item.UserID);
                                updateCommand.Parameters.AddWithValue("@BookID", item.BookID);
                                updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                                updateCommand.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                        else
                        {
                            reader.Close();
                            // Item doesn't exist, insert new record
                            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Cart (UserID, BookID, Quantity) VALUES (@UserID, @BookID, 1)", connection))
                            {

                                insertCommand.Parameters.AddWithValue("@UserID", item.UserID);
                                insertCommand.Parameters.AddWithValue("@BookID", item.BookID);
                                insertCommand.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }

                }

            }

            return Ok(new
            {
                message = "Item inserted to cart successfully"
            });
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public async Task<ActionResult> RemoveFromCart(Cart item)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();
                // Check if item exists in cart
                using (SqlCommand command = new SqlCommand("SELECT * FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", item.UserID);
                    command.Parameters.AddWithValue("@BookID", item.BookID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // Item doesn't exist, return error
                            return BadRequest("Item not found in cart");
                        }
                    }
                }
                // Item exists, delete it from cart
                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@UserID", item.UserID);
                    deleteCommand.Parameters.AddWithValue("@BookID", item.BookID);
                    deleteCommand.ExecuteNonQuery();
                }
                connection.Close();
            }
            return Ok(new
            {
                message = "Item removed from cart"
            });
        }

        [HttpPut]
        [Route("api/cart/decrease")]
        public async Task<ActionResult> DecreaseFromCart(Cart item)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Check if item already exists in cart
                using (SqlCommand command = new SqlCommand("SELECT * FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", item.UserID);
                    command.Parameters.AddWithValue("@BookID", item.BookID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Item exists in cart, decrease quantity
                            int quantity = (int)reader["Quantity"] - 1;
                            if (quantity > 0)
                            {
                                // Quantity still > 0, update record
                                reader.Close();
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Cart SET Quantity = @Quantity WHERE UserID = @UserID AND BookID = @BookID", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@UserID", item.UserID);
                                    updateCommand.Parameters.AddWithValue("@BookID", item.BookID);
                                    updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Quantity = 0, delete record
                                reader.Close();
                                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection))
                                {
                                    deleteCommand.Parameters.AddWithValue("@UserID", item.UserID);
                                    deleteCommand.Parameters.AddWithValue("@BookID", item.BookID);
                                    deleteCommand.ExecuteNonQuery();
                                }
                            }
                            return Ok();
                        }
                        else
                        {
                            // Item doesn't exist in cart, return error
                            return NotFound();
                        }
                    }
                }
            }
        }
    }
}
