using System.Linq;
using Microsoft.Web.WebPages.OAuth;
using WebService.Contracts;
using WebService.Models;

namespace WebService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public bool CreateUser(AccountModels.RegisterExternalLoginModel model, string provider, string providerUserId)
        {
            using (AccountModels.UsersContext db = new AccountModels.UsersContext())
            {
                AccountModels.UserProfile user =
                    db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                if (user == null)
                {
                    user = new AccountModels.UserProfile { UserName = model.UserName };
                    db.UserProfiles.Add(user);
                    db.SaveChanges();

                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);

                    return true;
                }

            }
            return false;
        }

        public int GetIdByUserName(string userName)
        {
            AccountModels.UserProfile user;
            using (AccountModels.UsersContext db = new AccountModels.UsersContext())
            {
                user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName);

            }
            return user.UserId;
        }
    }
}