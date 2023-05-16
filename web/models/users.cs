using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace web.Models
{
    public partial class Users
    {
        public Users()
        {
            Log = new HashSet<Log>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Parol { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<Log> Log { get; set; }
    }
}
