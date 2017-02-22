using WebService.Models;

namespace WebService.Contracts
{
    public interface IAccountRepository
    {
        bool CreateUser(AccountModels.RegisterExternalLoginModel model, string provider, string providerUserId);
        int GetIdByUserName(string userName);
    }
}
