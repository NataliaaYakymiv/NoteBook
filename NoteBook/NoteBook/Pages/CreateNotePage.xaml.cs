using NoteBook.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Services;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace NoteBook.Pages
{
    public partial class CreateNotePage : ContentPage
    {
        public INotesService NotesService { get; private set; }
        private IMediaPicker _mediaPicker;
        private MediaFile _mediaFile;

        private void Setup()
        {
            if (_mediaPicker != null)
            {
                return;
            }

            var device = Resolver.Resolve<IDevice>();
 
            _mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
        }

        private CreateNotePage()
        {
            Title = "Create note";
            InitializeComponent();
            StateLabel.Text = string.Empty;
        }

        public CreateNotePage(INotesService notesService) : this()
        {
            NotesService = notesService;
        }

        public async void OnCreateNote(object sender, EventArgs e)
        {
            var note = new NoteModel
            {
                NoteName = NoteNameEntry.Text,
                NoteText = NoteTextEntry.Text,
                MediaFile = _mediaFile
            };

            try
            {
                ActivityIndicatorCreateNote.IsRunning = true;
                ActivityIndicatorCreateNote.IsVisible = true;
                NoteNameEntry.IsEnabled = false;
                NoteTextEntry.IsEnabled = false;
                CreateBtn.IsVisible= false;

                var result = await NotesService.CreateNote(note);

                ActivityIndicatorCreateNote.IsRunning = false;
                ActivityIndicatorCreateNote.IsVisible = false;
                NoteNameEntry.IsEnabled = true;
                NoteTextEntry.IsEnabled = true;
                CreateBtn.IsVisible = true;

                if (result)
                {
                    NoteNameEntry.Text = string.Empty;
                    NoteTextEntry.Text = string.Empty;

                    await Navigation.PopAsync();
                }
                else
                {
                    StateLabel.Text = "Something wrong, try again";
                }
            }
            catch (HttpRequestException)
            {
                await Navigation.PopToRootAsync();
            }
            catch (InvalidOperationException exception)
            {
                StateLabel.Text = exception.Message;
            }
            catch (InvalidCastException exception)
            {
                StateLabel.Text = exception.Message;
            }
        }

        private async void OnSelectImage(object sender, EventArgs eventArgs)
        {
            Setup();
            Image.Source = null;
            try
            {
                _mediaFile = await this._mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                { 
                    
                    DefaultCamera = CameraDevice.Front,
                    MaxPixelDimension = 400,
                    

                });
                Image.Source = ImageSource.FromStream(() => _mediaFile.Source);
                Image.IsVisible = true;
            }
            catch (System.Exception ex)
            {
                StateLabel.Text = "some kekxeption";
            }

        }
    }
}
