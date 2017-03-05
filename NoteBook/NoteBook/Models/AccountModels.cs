namespace NoteBook.Models
{
    public class AccountModels
    {
        public class LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        public class RegisterModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }

        }
    }
}
