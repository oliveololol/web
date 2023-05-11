using Microsoft.EntityFrameworkCore;

namespace web.models
{
    
    
        public class usersContext : DbContext
        {
            

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

