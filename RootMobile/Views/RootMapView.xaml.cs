using CommunityToolkit.Maui.Views;
using Maui.GoogleMaps;
using RootMobile.Services;
using RootMobile.Views.Templates;
using SkiaSharp;
using System.Text.Json;
using RootMobile.Models;

namespace RootMobile.Views;

public partial class RootMapView : ContentPage
{
    private DataService _dataService;

    private Dictionary<Pin, PlantPinDataModel> _pinDataMap = new();

    private string _mapStyleJson = @"
[
  {
    ""featureType"": ""poi"",
    ""elementType"": ""all"",
    ""stylers"": [
      { ""visibility"": ""off"" }
    ]
  },
  {
    ""featureType"": ""transit"",
    ""elementType"": ""all"",
    ""stylers"": [
      { ""visibility"": ""off"" }
    ]
  },
  {
    ""featureType"": ""road"",
    ""elementType"": ""labels"",
    ""stylers"": [
      { ""visibility"": ""on"" }
    ]
  },
  {
    ""featureType"": ""administrative"",
    ""elementType"": ""labels"",
    ""stylers"": [
      { ""visibility"": ""on"" }
    ]
  }
]";


    public RootMapView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        // ТИМЧАСОВО: Розкоментуйте цей рядок, щоб видалити всі збережені дані
        if (File.Exists(_dbPath)) File.Delete(_dbPath);

        mymap.MapStyle = MapStyle.FromJson(_mapStyleJson);

        mymap.UiSettings.TiltGesturesEnabled = false;
        mymap.UiSettings.RotateGesturesEnabled = true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        string accessToken = await SecureStorage.Default.GetAsync("authToken");
        string refreshToken = await SecureStorage.Default.GetAsync("refreshToken");
        if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
        {
            Shell.Current.Navigation.PushModalAsync(new LaunchView(_dataService), true);
        }
        
        PermissionStatus result = await CheckAndRequestLocationPermission();

        if (result == PermissionStatus.Granted)
        {
            mymap.UiSettings.MyLocationButtonEnabled = true;
            mymap.MyLocationEnabled = true;
            // Завантажуємо тільки те, що є в пам'яті
            LoadSavedPins();

            Location location = await Geolocation.Default.GetLocationAsync();

            if (location != null)
            {
                Position myposition = new Position(location.Latitude, location.Longitude);

                await mymap.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                           new CameraPosition(myposition, 17d, 0d, 0d)));

            }
        }
    }

    private List<PlantPinDataModel> _savedPins = new();
    private string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "pins.json");

    // 3. Коли натиснули на Пін на карті
    private async void OnPinClicked(object sender, PinClickedEventArgs e)
    {
        e.Handled = true; // Вимикаємо стандартну бульбашку Google

        if (_pinDataMap.ContainsKey(e.Pin))
        {
            var data = _pinDataMap[e.Pin];

            // Викликаємо наш новий Popup і передаємо йому дані
            var popup = new ShowPinDetailPopup(data);
            await this.ShowPopupAsync(popup);
        }
    }

    // БЕЗПЕЧНИЙ МЕТОД СТВОРЕННЯ МАРКЕРА
    private BitmapDescriptor CreateRoundMarker(string imagePath)
    {
        try
        {
            // 1. Перевірка на існування та розмір файлу
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return BitmapDescriptorFactory.DefaultMarker(Colors.Green);

            var fileInfo = new FileInfo(imagePath);
            if (fileInfo.Length == 0)
                return BitmapDescriptorFactory.DefaultMarker(Colors.Green);

            // 2. Читаємо файл безпечно (тільки для читання)
            using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sourceBitmap = SKBitmap.Decode(fs);

            if (sourceBitmap == null)
                return BitmapDescriptorFactory.DefaultMarker(Colors.Green);

            // 3. Малювання (SkiaSharp)
            int size = 150;
            int strokeWidth = 10;
            var info = new SKImageInfo(size, size);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            var center = size / 2f;
            var radius = (size - strokeWidth) / 2f;

            using var clipPath = new SKPath();
            clipPath.AddCircle(center, center, radius);

            canvas.Save();
            canvas.ClipPath(clipPath, SKClipOperation.Intersect, true);

            float scale = Math.Max((float)size / sourceBitmap.Width, (float)size / sourceBitmap.Height);
            float x = (size - sourceBitmap.Width * scale) / 2f;
            float y = (size - sourceBitmap.Height * scale) / 2f;
            var destRect = new SKRect(x, y, x + sourceBitmap.Width * scale, y + sourceBitmap.Height * scale);

            canvas.DrawBitmap(sourceBitmap, destRect);
            canvas.Restore();

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = strokeWidth,
                IsAntialias = true
            };
            canvas.DrawCircle(center, center, radius, paint);

            // 4. КОНВЕРТАЦІЯ (Найважливіша частина для Android)
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            if (data == null) return BitmapDescriptorFactory.DefaultMarker(Colors.Green);

            // Копіюємо в MemoryStream, щоб уникнути помилок доступу в Android
            var ms = new MemoryStream();
            data.SaveTo(ms);
            ms.Position = 0;

            return BitmapDescriptorFactory.FromStream(ms);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Критична помилка маркера: {ex.Message}");
            return BitmapDescriptorFactory.DefaultMarker(Colors.Red);
        }
    }

    private async void AddPinToMap(PlantPinDataModel data)
    {
        // Отримуємо локальний шлях (якщо це URL - завантажимо)
        string localPath = await GetLocalPathForImage(data.Image);
        // Завжди виконуємо операції з UI (картою) в головному потоці
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var descriptor = CreateRoundMarker(localPath);

                var pin = new Pin
                {
                    Label = data.Name,
                    Position = new Position(data.Latitude, data.Longitude),
                    Type = PinType.Place,
                    Icon = descriptor ?? BitmapDescriptorFactory.DefaultMarker(Colors.Green)
                };

                _pinDataMap[pin] = data; // Зберігаємо зв'язок
                mymap.Pins.Add(pin);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка додавання піна: {ex.Message}");
            }
        });
    }

    private async void LoadSavedPins()
    {
        try
        {
            // Отримуємо список із Supabase через сервіс
            var pins = await _dataService.GetAllPlantPinsAsync();

            if (pins != null)
            {
                foreach (var pinData in pins)
                {
                    AddPinToMap(pinData);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading pins: {ex.Message}");
        }
    }

    private async void OnCreatePinClicked(object sender, EventArgs e)
    {
        var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
        if (location == null) return;

        var popup = new CreatePlantPinPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is PlantPinDataModel newPin)
        {
            try
            {
                var session = _dataService.SupabaseClient.Auth.CurrentSession;
                if (session == null)
                {
                    await DisplayAlert("Error", "Ви не авторизовані", "OK");
                    return;
                }

                // Зберігаємо локальний шлях для негайного відображення
                string localPathForNow = newPin.Image;

                newPin.UserId = Guid.Parse(session.User.Id);
                newPin.Latitude = location.Latitude;
                newPin.Longitude = location.Longitude;
                newPin.Created = DateTime.Now;
                newPin.Subcategory = "Smth";


                // 1. Завантаження в Storage
                string publicUrl = await _dataService.UploadPlantImageAsync(localPathForNow);

                if (!string.IsNullOrEmpty(publicUrl))
                {
                    newPin.Image = publicUrl;

                    // 2. Збереження в Таблицю
                    bool success = await _dataService.InsertPlantPinAsync(newPin);

                    if (success)
                    {
                        // Трюк: для AddPinToMap підсовуємо локальний шлях, щоб не чекати завантаження з мережі
                        var pinToDraw = new PlantPinDataModel
                        {
                            Name = newPin.Name,
                            Latitude = newPin.Latitude,
                            Longitude = newPin.Longitude,
                            Image = localPathForNow, // Локальний файл
                            Category = newPin.Category,
                            Description = newPin.Description,
                        };

                        AddPinToMap(pinToDraw);
                        await DisplayAlert("Успіх", "Збережено в базу!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Помилка", "Дані не внесені в таблицю map_points. Перевірте RLS політики.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Помилка", "Фото не завантажено в Bucket. Перевірте назву бакета (plant_images) та права доступу.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Критична помилка", ex.Message, "OK");
            }
        }
    }

    private async Task<string> GetLocalPathForImage(string pathOrUrl)
    {
        if (string.IsNullOrEmpty(pathOrUrl)) return null;

        // Якщо це вже локальний файл — просто повертаємо його
        if (!pathOrUrl.StartsWith("http")) return pathOrUrl;

        try
        {
            // Якщо це URL — качаємо у тимчасову папку (кеш)
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(pathOrUrl);

            var fileName = Path.GetFileName(new Uri(pathOrUrl).LocalPath);
            var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(localPath, bytes);
            return localPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error downloading image: {ex.Message}");
            return null;
        }
    }

    async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status;
    }
}