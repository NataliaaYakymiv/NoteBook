using Xamarin.Forms;
using NoteBook.Contracts;
using NoteBook.Services;

namespace NoteBook
{
    public partial class App : Application
    {
        public static NotesItemManager NotesItemManager { get; private set; }

        public const string DATABASE_NAME = "notes1.db";
        public static INotesService database;
        public static INotesService Database
        {
            get
            {
                if (database == null)
                {
                    database = new NoteService(DATABASE_NAME);
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            NotesItemManager = new NotesItemManager(new RemoteNotesService());

            MainPage = new NavigationPage(new Pages.MainPage());

            var a = App.Database.GetAllNotes();
            foreach (var model in a)
            {
                App.Database.DeleteNote(model);
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
