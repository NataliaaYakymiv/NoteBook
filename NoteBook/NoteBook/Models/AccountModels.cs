namespace NoteBook.Models
{
    public class AccountModels
    {
        public class UserProfile
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }

        public class OAuthMembership
        {
            public string Provider { get; set; }
            public string ProviderUserId { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
        }

        public class RegisterExternalLoginModel
        {
            public string UserName { get; set; }
            public string ExternalLoginData { get; set; }
            public string Email { get; set; }
        }

        public class LocalPasswordModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

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

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }

        }

        public class ExternalLogin
        {
            public string Provider { get; set; }
            public string ProviderDisplayName { get; set; }
            public string ProviderUserId { get; set; }
        }
    }
}
