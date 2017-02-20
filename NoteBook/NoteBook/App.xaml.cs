using Xamarin.Forms;
using NoteBook.Contracts;
using NoteBook.Servises;

namespace NoteBook
{
    public partial class App : Application
    {
        public static NotesItemManager NotesItemManager { get; private set; }


        public App()
        {
            InitializeComponent();

            NotesItemManager = new NotesItemManager(NotesService.GetService());

            MainPage = new NavigationPage(new Pages.MainPage());
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
