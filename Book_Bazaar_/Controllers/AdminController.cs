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
        [Route("api/users")]
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

        [HttpPost]
        [Route("api/users/{userId}/delete-user")]
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


    }
}
