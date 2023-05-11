namespace web.models
{
    public class Users
    {
       /* public class AppDBContext : DbContext
        {
            
            public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
            {
              
            }
            public DbSet<Users> Users { get; set; }
            
        }*/
        public int id { get; set; }
        public string login { get; set; }
        public string parol { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
     

    }
}
