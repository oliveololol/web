using web.Models;

namespace web.models
{
    public class UserWithToken : Users
    {       

      public string Token { get; set; }
        public UserWithToken (Users user)
        {
            this.Id = user.Id;
            this.Login = user.Login;
            this.Name = user.Name;
            this.Surname = user.Surname;

        }
    }
}
