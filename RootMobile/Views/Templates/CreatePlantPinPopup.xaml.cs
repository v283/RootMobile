using CommunityToolkit.Maui.Views;

using Maui.GoogleMaps;
using RootMobile.Services;
using SkiaSharp;
using System.Text.Json;
using RootMobile.Models;

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

    private void OnFinishPinClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlantNameEntry.Text) || string.IsNullOrEmpty(_tempImagePath))
        {
            // Додайте DisplayAlert або візуальну підказку
            return;
        }

        var result = new PlantPinDataModel
        {
            Name = PlantNameEntry.Text.Trim(),
            Category = CategoryPicker.SelectedItem?.ToString(),
            Description = CommentEntry.Text?.Trim(),
            Image = _tempImagePath // Поки що це локальний шлях до файлу
        };

        Close(result);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // Повертаємо null, якщо скасовано
    }
}