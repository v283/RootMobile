using CommunityToolkit.Maui.Views;
using Maui.GoogleMaps;
using RootMobile.Models;
using RootMobile.Services;
using SkiaSharp;
using System.Text.Json;

namespace RootMobile.Views.Templates;

public partial class CreatePlantPinPopup : Popup
{
    private string _tempImagePath;
    private readonly DataService _dataService;
    private List<CategoriesMapModel> _categories;
    private readonly OpenAIService _aiService;


    public CreatePlantPinPopup(IDataService dataService, IOpenAIService aiService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        _aiService = (OpenAIService)aiService; // Зберігаємо посилання на ШІ
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

    private byte[] CompressImage(string path)
    {
        using var original = SKBitmap.Decode(path);
        if (original == null) return null;

        int maxSide = 1024; // Для ШІ більше не потрібно
        int width = original.Width;
        int height = original.Height;

        if (width > maxSide || height > maxSide)
        {
            double ratio = (double)Math.Max(width, height) / maxSide;
            width = (int)(width / ratio);
            height = (int)(height / ratio);
        }

        using var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70); // Якість 70% ідеальна для передачі
        return data.ToArray();
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo == null) return;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // 1. Зберігаємо локально для карти
            var localPath = Path.Combine(FileSystem.AppDataDirectory, Guid.NewGuid().ToString() + ".jpg");
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(localPath))
                await stream.CopyToAsync(newStream);

            // 2. Стискаємо зображення
            byte[] compressedImage = CompressImage(localPath);

            // 3. Викликаємо ваш OpenAIService
            string aiResult = await _aiService.IdentifyPlantAsync(compressedImage);

            if (aiResult.ToUpper().Contains("NOT_PLANT"))
            {
                await App.Current.MainPage.DisplayAlert("Помилка", "ШІ не впізнав рослину. Зробіть фото чіткішим.", "ОК");
                return;
            }

            // Успіх: заповнюємо дані
            _tempImagePath = localPath;
            PreviewImage.Source = ImageSource.FromFile(localPath);
            PlantNameEntry.Text = aiResult;
            CameraPlaceholder.IsVisible = false;

            // Можна відразу згенерувати опис через той же ШІ
            OnAiHelpClicked(null, null);
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("API Error", "Не вдалося проаналізувати фото", "ОК");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnFinishPinClicked(object sender, EventArgs e)
    {
        // 1. Спочатку базова перевірка (валідація), щоб не ганяти запити до ШІ даремно
        if (string.IsNullOrWhiteSpace(PlantNameEntry.Text) || string.IsNullOrEmpty(_tempImagePath))
        {
            await App.Current.MainPage.DisplayAlert("Помилка", "Заповніть назву та додайте фото", "ОК");
            return;
        }

        try
        {
            // 2. Вмикаємо індикатор завантаження
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            PopupBorder.Opacity = 0.5; // Візуально "заблокуємо" форму

            // Очищуємо історію перед перевіркою, щоб ШІ не плутав контекст
            _aiService.ClearHistory();

            // 3. Перевірка на матюки через ШІ
            bool hasProfanity = await _aiService.ContainsProfanity(PlantNameEntry.Text + " " + (CommentEntry.Text ?? ""));

            if (hasProfanity)
            {
                await App.Current.MainPage.DisplayAlert("Модерація", "Будь ласка, використовуйте ввічливі слова.", "ОК");
                return;
            }

            // 4. Формуємо результат
            var selectedCat = CategoryPicker.SelectedItem as CategoriesMapModel;

            var result = new PlantPinModel // Переконайтеся, що назва моделі правильна
            {
                Name = PlantNameEntry.Text.Trim(),
                Category = selectedCat?.Name ?? "Не обрано",
                Description = CommentEntry.Text?.Trim(),
                Image = _tempImagePath
            };

            Close(result);
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Помилка", "Сталася помилка при збереженні", "ОК");
        }
        finally
        {
            // 5. Вимикаємо індикатор у будь-якому випадку
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            PopupBorder.Opacity = 1;
        }
    }

    private async void OnAiHelpClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlantNameEntry.Text))
        {
            await App.Current.MainPage.DisplayAlert("Помилка", "Спочатку введіть назву рослини", "ОК");
            return;
        }

        try 
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            CommentEntry.Placeholder = "ШІ генерує поради...";

            // Очищуємо історію для нового питання
            _aiService.ClearHistory();

            string prompt = $"Ти експерт-садівник. Напиши дуже коротко (до 3 речень) поради по догляду та цікавий факт про рослину: {PlantNameEntry.Text}";

            string aiResult = await _aiService.AskQuestion(prompt);
            CommentEntry.Text = aiResult;
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // Повертаємо null, якщо скасовано
    }
}