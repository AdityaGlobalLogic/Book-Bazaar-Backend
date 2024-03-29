﻿using Book_Bazaar_.Models.Auth;
using Book_Bazaar_.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        [Route("GetBooks")]
        public async Task<ActionResult> GetBooks()
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
                                BookID = (Guid)reader["BookID"],
                                Title = (string)reader["Title"],
                                Description = (string)reader["Description"],
                                AuthorName = (string)reader["AuthorName"],
                                Price = (decimal)reader["Price"],
                                Quantity= (int)reader["Quantity"],
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

        [HttpGet]
        [Route("{BookID}/FilterBookById")]
        public async Task<ActionResult> FilterBookById(Guid BookID)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Books where BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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
                            connection.Close();
                            return Ok(book);
                        }
                        else
                        {
                            return BadRequest("Book not found");
                        }
                    }
                }
            }
        }

        [HttpGet]
        [Route("GetTrendingBooks")]
        public async Task<ActionResult> GetTrendingBooks()
        {
            List<Books> books = new List<Books>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Retrieve list of books from database
                using (SqlCommand command = new SqlCommand("SELECT * FROM Books where Rating >=4.0", connection))
                {
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

        [HttpGet]
        [Route("{CategoryID}/FilterBooksByCategory")]
        public async Task<ActionResult> FilterBooksByCategory(Guid CategoryID)
        {
            List<Books> books = new List<Books>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("MyCon").ToString()))
            {
                connection.Open();

                // Retrieve list of books from database
                using (SqlCommand command = new SqlCommand("SELECT * FROM Books where CategoryID = @CategoryID", connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", CategoryID);
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
    }
}

