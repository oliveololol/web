using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using web.models;

namespace web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        /*private readonly IConfiguration _configuration;
        public userController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        static public SqlConnection myCon;
        */
        
        /*  
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                select id,login,parol,name,surname from
                dbo.users
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("userDB");
            myCon = new SqlConnection(sqlDataSource);
            SqlDataReader myReader;

            myCon.Open();
            using (SqlCommand myCommand = new SqlCommand(query, myCon))
            {
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                myCon.Close();
            }

            return new JsonResult(table);
        }
        */

        public class usersContext : DbContext
        {
            private string connectionString = "userDB";

            public usersContext(DbContextOptions<usersContext> options) : base()
            { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("userDB");
                }
            }

            public DbSet<Users> users { get; set; }
            public DbSet<Log> log { get; set; }
            public DbSet<eventt> eventt { get; set; }
        }
    }
}
