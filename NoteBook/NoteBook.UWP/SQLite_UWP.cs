using System.IO;
using Windows.Storage;
using NoteBook.Contracts;
using NoteBook.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_UWP))]
namespace NoteBook.UWP
{
    public class SQLite_UWP : ISQLite
    {
        public SQLite_UWP() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            return path;
        }
    }
}
