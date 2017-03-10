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
    public partial class UpdateNotePage : ContentPage
    {
        public INotesService NotesService { get; private set; }
        public NoteModel NoteModel { get; private set; }

        private IMediaPicker _mediaPicker;
        private MediaFile _mediaFile;

        private void ImagepickerSetup()
        {
            if (_mediaPicker != null)
            {
                return;
            }

            var device = Resolver.Resolve<IDevice>();

            _mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
        }

        private UpdateNotePage()
        {
            Title = "Update note";
            InitializeComponent();
        }
        
        public void OnUpdateNoteCheck(object sender, EventArgs e)
        {
            switch (!(string.IsNullOrEmpty(NoteNameEntry.Text) && string.IsNullOrEmpty(NoteTextEntry.Text)))
            {
                case false:
                    StateLabel.Text = "Please, write something.";
                    break;
                case true:
                    OnUpdateNote(this, EventArgs.Empty);
                    break;
                default:
                    OnUpdateNote(this, EventArgs.Empty);
                    break;
            }
        }

        public UpdateNotePage(INotesService notesService) : this()
        {
            NotesService = notesService;
            StateLabel.Text = string.Empty;
        }

        public void SetNoteModel(NoteModel model)
        {
            ForceLayout();
            NoteModel = model;

            if (model.Image != null)
            {
                Image.Source = model.Image;
                Image.IsVisible = true;
                RemoveImageBtn.IsEnabled = true;
            }
            else
            {
                Image.IsVisible = false;
                RemoveImageBtn.IsEnabled = false;
            }

            NoteNameEntry.Text = model.NoteName;
            NoteTextEntry.Text = model.NoteText;
            CreatedLabel.Text = "Created: " + model.Create;
            UpdatedLabel.Text = !string.IsNullOrEmpty(model.Update) ? "Last update: " + model.Update : string.Empty;
        }

        public async void OnUpdateNote(object sender, EventArgs e)
        {
            NoteModel.NoteName = NoteNameEntry.Text;
            NoteModel.NoteText = NoteTextEntry.Text;
            NoteModel.MediaFile = _mediaFile;

            try
            {
                #region Enabling views
                ActivityIndicatorUdpateNote.IsRunning = true;
                ActivityIndicatorUdpateNote.IsVisible = true;
                UpdateBtn.IsEnabled = false;
                #endregion

                var result = await NotesService.UpdateNote(NoteModel);

                #region Disabling Views
                ActivityIndicatorUdpateNote.IsRunning = false;
                ActivityIndicatorUdpateNote.IsVisible = false;
                UpdateBtn.IsEnabled = true;
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
            ImagepickerSetup();
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
            catch (Exception ex)
            {
                StateLabel.Text = ex.Message;
            }
            OnAppearing();
        }

        public async void OnDeleteNote(object sender, EventArgs e)
        {
            NoteNameEntry.Text = string.Empty;
            NoteTextEntry.Text = string.Empty;
            await NotesService.DeleteNote(NoteModel);
            await Navigation.PopAsync();
        }

        private async void ShowDeleteDialog(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete", "Do you want to delete this note?", "Yes", "No");
            if (answer)
            {
                OnDeleteNote(this, EventArgs.Empty);
            }
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
