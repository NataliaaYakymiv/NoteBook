using System.Net.Http;
using System.Threading.Tasks;
using NoteBook.Models;

namespace NoteBook.Contracts
{
    public interface IAccountService
    {
        Task<bool> Login(AccountModels.LoginModel credentials);
        Task<bool> ExternalLogin(string url);
        Task<bool> Register(AccountModels.RegisterModel credentials);
        Task Logout();
        bool IsLoged();
    }
}