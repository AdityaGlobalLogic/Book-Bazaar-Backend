
using Book_Bazaar_.Models.Auth;
using Book_Bazaar_.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;

namespace Book_Bazaar_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> SignUp(RegisterModel user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();
                // Check if email already exists
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return BadRequest("Email already exists");
                    }
                }
                // Create user and add to database
                using (SqlCommand command = new SqlCommand("INSERT INTO Users (FirstName, LastName, Password, Email) VALUES (@FirstName, @LastName, @Password, @Email)", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return Ok(new
            {
                message = "User registered successfully"
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();
                // Find user by email and password
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", connection))
                {
                    
                    command.Parameters.AddWithValue("@Email", loginModel.Email);
                    //command.Parameters.AddWithValue("@Password", login_pass);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string pass = (string)(reader)["Password"];
                            bool isvalid = BCrypt.Net.BCrypt.Verify(loginModel.Password, pass);
                            if (isvalid)
                            {
                                Users authticatedUser = new Users
                                {
                                    UserID = (Guid)reader["UserID"],
                                    Email = (string)reader["Email"],
                                    IsVendor = (bool)reader["IsVendor"],
                                    FirstName = (string)reader["FirstName"],
                                    LastName = (string)reader["LastName"]
                                };
                                connection.Close();
                                return Ok(new
                                {
                                    message = "Logged In Successfull",
                                    authticatedUser
                                });
                                
                            }
                            else
                            {
                                connection.Close();
                                return BadRequest("Invalid Email or Password");

                            }

                        }
                        connection.Close();
                        return BadRequest("Invalid Email or Password");
                    }
                    
                }
            }
        }
    }
}
