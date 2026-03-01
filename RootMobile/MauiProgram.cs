using Maui.GoogleMaps.Hosting;
using Microsoft.Extensions.Logging;

namespace RootMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();

#endif
#if ANDROID
        builder.UseGoogleMaps();
#elif IOS
        builder.UseGoogleMaps("AIzaSyAKbeCntKNiPUHflmWGV__6ZPT22ppkLdM");
#endif
        return builder.Build();
    }
}