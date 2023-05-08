using Book_Bazaar_.Models;
using Book_Bazaar_.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("{userId}/GenerateOrder")]
        public async Task<ActionResult> GenerateOrder(Guid userId, [FromBody] OrderModel item)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                await connection.OpenAsync();

                // Start a new transaction to ensure data consistency
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert order information into Orders table
                        using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Orders (UserID, OrderTotal, OrderStatus, Address, City, State, Pincode) VALUES (@UserID, @OrderTotal, @OrderStatus, @Address, @City, @State, @Pincode); SELECT SCOPE_IDENTITY()", connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@UserID", item.UserID);
                            insertCommand.Parameters.AddWithValue("@OrderTotal", item.OrderTotal);
                            insertCommand.Parameters.AddWithValue("@OrderStatus", "Pending");
                            insertCommand.Parameters.AddWithValue("@Address", item.Address);
                            insertCommand.Parameters.AddWithValue("@City", item.City);
                            insertCommand.Parameters.AddWithValue("@State", item.State);
                            insertCommand.Parameters.AddWithValue("@Pincode", item.Pincode);
                            Guid orderId = (Guid) insertCommand.ExecuteScalar();

                            List<CartTable> books = new List<CartTable>();

                            using (SqlCommand command = new SqlCommand("SELECT * FROM Cart WHERE UserID = @UserID", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@UserID", userId);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    
                                    while (reader.Read())
                                    {
                                        CartTable book = new CartTable
                                        {
                                            BookID = (Guid)reader["BookID"],
                                            Quantity = (int)reader["Quantity"]
                                            /*Rating = (decimal)reader["Rating"],*/
                                        };
                                        books.Add(book);
                                    }
                                }
                            }

                            // Update book quantities and insert book information into OrderItems table
                            foreach (var book in books)
                            {
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Books SET Quantity = Quantity - @Quantity WHERE BookID = @BookID", connection, transaction))
                                {
                                    updateCommand.Parameters.AddWithValue("@Quantity", book.Quantity);
                                    updateCommand.Parameters.AddWithValue("@BookId", book.BookID);
                                    await updateCommand.ExecuteNonQueryAsync();
                                }

                                using (SqlCommand command = new SqlCommand("INSERT INTO OrderItems (OrderID, BookID, Quantity) VALUES (@OrderID, @BookID, @Quantity)", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@OrderID", orderId);
                                    command.Parameters.AddWithValue("@BookId", book.BookID);
                                    command.Parameters.AddWithValue("@Quantity", book.Quantity);
                                    await command.ExecuteNonQueryAsync();
                                }

                                // Delete book from cart table
                                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM Cart WHERE UserID = @UserID AND BookID = @BookID", connection, transaction))
                                {
                                    deleteCommand.Parameters.AddWithValue("@UserId", userId);
                                    deleteCommand.Parameters.AddWithValue("@BookId", book.BookID);
                                    await deleteCommand.ExecuteNonQueryAsync();
                                }
                            }

                            // Commit the transaction and return success message
                            transaction.Commit();
                            return Ok(new
                            {
                                message = "Order placed successfully"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction and return error message
                        transaction.Rollback();
                        return BadRequest("Failed to generate order: " + ex.Message);
                    }
                }
            }
        }

    }
}
