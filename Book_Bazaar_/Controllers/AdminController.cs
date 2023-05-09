using Book_Bazaar_.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult> GetUsers()
        {
            List<Users> users = new List<Users>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Retrieve list of books from database
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Users u1 = new Users
                            {
                                UserID = (Guid)reader["UserID"],
                                Email = (string)reader["Email"],
                                IsVendor = (bool)reader["IsVendor"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"]
                            };

                            users.Add(u1);
                        }
                    }
                }
                connection.Close();
            }

            return Ok(users);
        }

        [HttpGet]
        [Route("{userId}/GetUserById")]
        public async Task<ActionResult> GetUserById(Guid userId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users where UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Users u1 = new Users
                            {
                                UserID = (Guid)reader["UserID"],
                                Email = (string)reader["Email"],
                                IsVendor = (bool)reader["IsVendor"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"]
                            };
                            connection.Close();
                            return Ok(u1);
                        }
                        else
                        {
                            return BadRequest("User not found");
                        }
                    }
                }
            }
        }


        [HttpPost]
        [Route("{userId}/DeleteUser")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                conn.Open();
                using (SqlCommand deleteCommand = new SqlCommand("delete from Users WHERE UserID = @UserID", conn))
                {

                    deleteCommand.Parameters.AddWithValue("@UserID", userId);
                    deleteCommand.ExecuteNonQuery();
                }
                conn.Close();
                return Ok(new
                {
                    message = "User removed from inventory."
                });
            }
        }

        [HttpDelete]
        [Route("DeleteAll")]

        public async Task<ActionResult> DeleteAll()
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                conn.Open();
                using (SqlCommand deleteCommand = new SqlCommand("delete from Users", conn))
                {

                    deleteCommand.ExecuteNonQuery();
                }
                conn.Close();
                return NoContent();
            }
        }
        [HttpDelete]
        [Route("DeleteMultipleUser")]
        
        public async Task<ActionResult> DeleteMultipleUser([FromBody] List<Guid> ids)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                conn.Open();
                using (SqlCommand deleteCommand = new SqlCommand($"DELETE FROM Users WHERE UserID IN ({string.Join(",", ids.Select(id => $"'{id}'"))})", conn))
                {

                    deleteCommand.ExecuteNonQuery();
                }
                conn.Close();
                return NoContent();
            }

        }

    }
}
