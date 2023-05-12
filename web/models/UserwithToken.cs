using web.Models;

namespace web.models
{
    public class UserWithToken : User
    {       

      public string Token { get; set; }
        public UserWithToken (User user)
        {
            this.Id = user.Id;
            this.Login = user.Login;
            this.Name = user.Name;
            this.Surname = user.Surname;

        }
    }
}
