using CommunityToolkit.Maui.Views;

using Maui.GoogleMaps;
using RootMobile.Services;
using SkiaSharp;
using System.Text.Json;

namespace RootMobile.Views.Templates;

public partial class CreatePlantPinPopup : Popup
{
    private string _tempImagePath;

    public CreatePlantPinPopup()
    {
        InitializeComponent();
        CategoryPicker.SelectedIndex = 0;

        // Анімація при відкритті
        Opened += async (s, e) =>
        {
            PopupBorder.TranslationY = 700;
            await PopupBorder.TranslateTo(0, 0, 400, Easing.SinOut);
        };
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync(); // Або CapturePhotoAsync
        if (photo != null)
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, Guid.NewGuid().ToString() + ".jpg");
            using var stream = await photo.OpenReadAsync();
            using var newStream = File.OpenWrite(localPath);
            await stream.CopyToAsync(newStream);

            _tempImagePath = localPath;
            PreviewImage.Source = ImageSource.FromFile(localPath);
            CameraPlaceholder.IsVisible = false;
        }
    }

    async void OnFinishPinClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(PlantNameEntry.Text) || string.IsNullOrEmpty(_tempImagePath))
        {
            //await DisplayAlert("Помилка", "Заповніть назву та додайте фото", "OK");
            return;
        }

        // Створюємо об'єкт результату (але без координат, їх додамо в основному вікні)
        var result = new PlantPinData
        {
            Name = PlantNameEntry.Text,
            Category = CategoryPicker.SelectedItem?.ToString(),
            Comment = CommentEntry.Text,
            ImagePath = _tempImagePath
        };

        Close(result);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // Повертаємо null, якщо скасовано
    }
}