using System.Net.Http;
using System.Threading.Tasks;
using NoteBook.Models;

namespace NoteBook.Contracts
{
    public interface IAccountService
    {
        Task<HttpResponseMessage> Login(AccountModels.LoginModel credentials);
        Task<HttpResponseMessage> Register(AccountModels.RegisterModel credentials);
    }
}