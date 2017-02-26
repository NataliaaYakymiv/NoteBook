using System.Net.Http;

namespace NoteBook.Contracts
{
    public interface IHttpAuth
    {
        HttpClient GetAuthHttpClient();
    }
}