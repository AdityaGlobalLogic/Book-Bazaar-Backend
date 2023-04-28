using Book_Bazaar_.Models.Auth;
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

        
    }
}
