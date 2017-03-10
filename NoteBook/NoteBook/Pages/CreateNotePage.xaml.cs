using NoteBook.Models;
using System;
using System.Net.Http;
using NoteBook.Contracts;
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

        private void ImapickerSetup()
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

        public void OnCreateNoteCheck(object sender, EventArgs e)
        {
            switch (!(string.IsNullOrEmpty(NoteNameEntry.Text) && string.IsNullOrEmpty(NoteTextEntry.Text)))
            {
                case false:
                    StateLabel.Text = "Please, write something.";
                    break;
                case true:
                    OnCreateNote(this, EventArgs.Empty);
                    break;
                default:
                    OnCreateNote(this, EventArgs.Empty);
                    break;
            }
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
                #region enabling views

                ActivityIndicatorCreateNote.IsRunning = true;
                ActivityIndicatorCreateNote.IsVisible = true;
                NoteNameEntry.IsEnabled = false;
                NoteTextEntry.IsEnabled = false;
                CreateBtn.IsVisible = false;

                #endregion

                var result = await NotesService.CreateNote(note);

                #region disabling views

                ActivityIndicatorCreateNote.IsRunning = false;
                ActivityIndicatorCreateNote.IsVisible = false;
                NoteNameEntry.IsEnabled = true;
                NoteTextEntry.IsEnabled = true;
                CreateBtn.IsVisible = true;

                #endregion

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
            ImapickerSetup();
            Image.Source = null;
            try
            {
                _mediaFile = await this._mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                {

                    DefaultCamera = CameraDevice.Front,
                    MaxPixelDimension = 400,


                });
                Image.Source = ImageSource.FromStream(() => _mediaFile.Source);
            }
            catch (Exception ex)
            {
                StateLabel.Text = ex.Message;
            }
            Image.IsVisible = true;
            RemoveImageBtn.IsEnabled = true;
        }

        private void OnRemoveImage(object sender, EventArgs eventArgs)
        {
            Image.Source = null;
            _mediaFile = null;
            Image.IsVisible = false;
            RemoveImageBtn.IsEnabled = false;
            OnAppearing();
        }

        private async void ShowRemoveImageDialog(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Removing image", "Do you want to remove this image from note?", "Yes", "No");
            if (answer)
            {
                OnRemoveImage(this, EventArgs.Empty);
            }
        }

    }
}
