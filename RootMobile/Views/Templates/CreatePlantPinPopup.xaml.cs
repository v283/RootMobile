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
    private readonly DataService _dataService;
    private List<CategoriesMapModel> _categories;

    public CreatePlantPinPopup(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        CategoryPicker.SelectedIndex = 0;

        // Завантажуємо категорії при ініціалізації
        LoadCategories();

        // Анімація при відкритті
        Opened += async (s, e) =>
        {
            PopupBorder.TranslationY = 700;
            await PopupBorder.TranslateTo(0, 0, 400, Easing.SinOut);
        };
    }

    private async void LoadCategories()
    {
        try
        {
            _categories = await _dataService.GetCategoriesAsync();
            CategoryPicker.ItemsSource = _categories;
        }
        catch (Exception ex)
        {
            //Debug.WriteLine(ex.Message);
        }
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
            return;

        var selectedCat = CategoryPicker.SelectedItem as CategoriesMapModel;

        var result = new PlantPinModel
        {
            Name = PlantNameEntry.Text.Trim(),
            Category = selectedCat?.Name ?? "Не обрано",
            Description = CommentEntry.Text?.Trim(),
            Image = _tempImagePath
        };

        Close(result);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // Повертаємо null, якщо скасовано
    }
}