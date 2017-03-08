using System.Linq;
using Microsoft.Web.WebPages.OAuth;
using WebService.Contracts;
using WebService.Models;

namespace WebService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private AccountModels.UsersContext _db;

        public AccountRepository()
        {
            _db = new AccountModels.UsersContext();
        }

        public AccountRepository(AccountModels.UsersContext db)
        {
            _db = db;
        }

        public bool CreateUser(AccountModels.RegisterExternalLoginModel model, string provider, string providerUserId)
        {
            AccountModels.UserProfile user =
                _db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

            if (user == null)
            {
                user = new AccountModels.UserProfile {UserName = model.UserName};
                _db.UserProfiles.Add(user);
                _db.SaveChanges();

                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);

                return true;
            }
            return false;
        }

        public int GetIdByUserName(string userName)
        {
            var user = _db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName);
            return user.UserId;
        }
    }
}