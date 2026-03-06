using Maui.GoogleMaps;
using SkiaSharp;
using System.Text.Json;
using RootMobile.Services;

namespace RootMobile.Views;

public partial class RootMapView : ContentPage
{
    private IDataService _dataService;
    string _tempImagePath;

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
        _dataService = dataService;
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
            // Завантажуємо тільки те, що є в пам'яті
            LoadSavedPins();

            Location location = await Geolocation.Default.GetLocationAsync();

            if (location != null)
            {
                Position myposition = new Position(location.Latitude, location.Longitude);

                await mymap.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                           new CameraPosition(myposition, 17d, 0d, 0d)));

                // ЦЕЙ ПІН МОЖЕ ВИКЛИКАТИ ПОМИЛКУ, ЯКЩО ФАЙЛУ "pin_marker" НЕМАЄ
                // ЗАМІНИМО ЙОГО НА БЕЗПЕЧНИЙ:
                Pin _pinA = new Pin()
                {
                    Icon = BitmapDescriptorFactory.DefaultMarker(Colors.Blue), // Безпечний варіант
                    Type = PinType.Place,
                    Label = "Ваша позиція",
                    Position = myposition
                };
                mymap.Pins.Add(_pinA);
            }
        }
    }

    private List<PlantPinData> _savedPins = new();
    private string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "pins.json");

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

    private void AddPinToMap(PlantPinData data)
    {
        // Завжди виконуємо операції з UI (картою) в головному потоці
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var descriptor = CreateRoundMarker(data.ImagePath);

                var pin = new Pin
                {
                    Label = data.Name,
                    Position = new Position(data.Latitude, data.Longitude),
                    Type = PinType.Place,
                    Icon = descriptor ?? BitmapDescriptorFactory.DefaultMarker(Colors.Green)
                };

                mymap.Pins.Add(pin);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка додавання піна: {ex.Message}");
            }
        });
    }

    private void LoadSavedPins()
    {
        try
        {
            if (File.Exists(_dbPath))
            {
                string json = File.ReadAllText(_dbPath);
                var loadedPins = JsonSerializer.Deserialize<List<PlantPinData>>(json);

                if (loadedPins != null)
                {
                    _savedPins = loadedPins;
                    mymap.Pins.Clear();
                    foreach (var pinData in _savedPins)
                    {
                        AddPinToMap(pinData);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading pins: {ex.Message}");
        }
    }

    // --- ЛОГІКА ФОТО ТА АНКЕТИ ---

    async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet("Оберіть фото", "Скасувати", null, "Зробити фото", "Обрати з галереї");
        if (action == "Зробити фото") await TakePhotoAsync();
        else if (action == "Обрати з галереї") await PickPhotoAsync();
    }

    async Task TakePhotoAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted) return;

            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null) await SaveAndProcessPhoto(photo);
        }
        catch (Exception ex) { await DisplayAlert("Помилка", ex.Message, "OK"); }
    }

    async Task PickPhotoAsync()
    {
        try
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null) await SaveAndProcessPhoto(photo);
        }
        catch (Exception ex) { await DisplayAlert("Помилка", ex.Message, "OK"); }
    }

    async Task SaveAndProcessPhoto(FileResult photo)
    {
        var localPath = Path.Combine(FileSystem.AppDataDirectory, Guid.NewGuid().ToString() + ".jpg");
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(localPath))
        {
            await stream.CopyToAsync(newStream);
        }
        _tempImagePath = localPath;
        MainThread.BeginInvokeOnMainThread(() => {
            PreviewImage.Source = ImageSource.FromFile(localPath);
        });
    }

    async void OnFinishPinClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(PlantNameEntry.Text) || string.IsNullOrEmpty(_tempImagePath))
        {
            await DisplayAlert("Помилка", "Заповніть назву та фото", "OK");
            return;
        }

        var location = await Geolocation.Default.GetLocationAsync();
        var newPinData = new PlantPinData
        {
            Id = Guid.NewGuid(),
            Name = PlantNameEntry.Text,
            ImagePath = _tempImagePath,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };

        _savedPins.Add(newPinData);
        File.WriteAllText(_dbPath, JsonSerializer.Serialize(_savedPins));
        AddPinToMap(newPinData);
        CloseAndClearForm();
    }

    private async void OnCreatePinClicked(object sender, EventArgs e)
    {
        var location = await Geolocation.Default.GetLocationAsync();
        if (location != null)
        {
            PinForm.IsVisible = true;
            await mymap.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                new CameraPosition(new Position(location.Latitude, location.Longitude), 17d, 0d, 0d)));
        }
    }

    private void OnCancelClicked(object sender, EventArgs e) => CloseAndClearForm();

    private void CloseAndClearForm()
    {
        PinForm.IsVisible = false;
        PlantNameEntry.Text = string.Empty;
        PreviewImage.Source = null;
        _tempImagePath = null;
    }

    async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status;
    }
}