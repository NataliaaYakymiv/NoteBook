using System;
using System.IO;
using NoteBook.Contracts;
using NoteBook.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace NoteBook.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFilename);

            return path;
        }
    }
}