using System.Threading.Tasks;
using NoteBook.Models;

namespace NoteBook.Contracts
{
    public interface IAccountService
    {
        Task<string> Login(FormCredentials credentials);
        Task<string> Register(FormCredentials credentials);
    }
}