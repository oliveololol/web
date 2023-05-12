using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace web.Models
{
    public partial class Log
    {
        public int Id { get; set; }
        public int? IdUsers { get; set; }
        public int? IdEvent { get; set; }

        public virtual Event IdEventNavigation { get; set; }
        public virtual User IdUsersNavigation { get; set; }
    }
}
