namespace NoteBook.Settings
{
    public class Settings
    {
        // if use Genymotion 2.2 and latest, you can access to localhost form this url http://10.0.3.2
        public static string Url { get; } = "http://192.168.88.116:81/";

        public static string DatabaseName { get; } = "notes1.db";

        public static string AccountPath { get; } = "api/Account/";
        public static string NotePath { get; } = "api/Notes/";

        public static string RegisterPath { get; } = AccountPath + "Register";
        public static string LoginPath { get; } = AccountPath + "Login";
        public static string LogoutPath { get; } = AccountPath + "Logout";
        public static string ExternalLoginPath { get; } = AccountPath + "Externallogin";
        public static string ExternalLoginCallbackPath { get; } = AccountPath + "Externallogincallback";
        public static string ExternalLoginConfirmationPath { get; } = AccountPath + "Externalloginconfirmation";
        public static string ExternalLoginFailurePath { get; } = AccountPath + "Externalloginfailure";
        public static string ExternalLoginFinalPath { get; } = AccountPath + "ExternalLoginFinal";

        public static string NoteGetAllNotesPath { get; } = NotePath + "GetAllNotes";
        public static string NoteCreatePath { get; } = NotePath + "CreateNote";
        public static string NoteUpdatePath { get; } = NotePath + "UpdateNote";
        public static string NoteDeletePath { get; } = NotePath + "DeleteNote";
        public static string NoteSyncPath { get; } = NotePath + "GetSyncNotes";
        public static string NoteAddImagePath { get; } = NotePath + "PostImage";
    }
}
