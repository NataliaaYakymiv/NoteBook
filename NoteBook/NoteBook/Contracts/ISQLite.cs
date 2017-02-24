namespace NoteBook.Contracts
{
    public interface ISQLite
    {
        string GetDatabasePath(string filename);
    }
}