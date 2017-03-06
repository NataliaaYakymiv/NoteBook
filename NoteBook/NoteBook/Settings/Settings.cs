namespace NoteBook
{
    public class Settings
    {
        //public static string Url { get; } = "http://6df9db01.ngrok.io/";
        public static string Url { get; } = "http://192.168.88.116:81/";

        public static string DatabaseName { get; } = "notes1.db";

        public static string RegisterPath { get; } = "api/Account/register";
        public static string LoginPath { get; } = "api/Account/login";
        public static string LogoutPath { get; } = "api/Account/logout";
        public static string ExternalLoginPath { get; } = "api/Account/externallogin";
        public static string ExternalLoginCallbackPath { get; } = "api/Account/externallogincallback";
        public static string ExternalLoginConfirmationPath { get; } = "api/Account/externalloginconfirmation";
        public static string ExternalLoginFailurePath { get; } = "api/Account/externalloginfailure";
        public static string ExternalLoginFinalPath { get; } = "api/account/ExternalLoginFinal";

        public static string NoteGetAllNotesPath { get; } = "api/Notes/GetAllNotes";
        public static string NoteCreatePath { get; } = "api/Notes/CreateNote";
        public static string NoteUpdatePath { get; } = "api/Notes/UpdateNote";
        public static string NoteDeletePath { get; } = "api/Notes/DeleteNote";
        public static string NoteSyncPath { get; } = "api/Notes/GetSyncNotes";
        public static string NoteAddImagePath { get; } = "api/Notes/PostImage";
        public static string NoteGetImagePath { get; } = "api/Notes/GetImage";
    }
}
