using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using Microsoft.Maui.Handlers;
using RootMobile.Constants;
using RootMobile.Services;
using RootMobile.ViewModels;
using RootMobile.Views;
#if ANDROID
using Android.Content.Res;
#endif
namespace RootMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseFFImageLoading()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        var url = AppConfig.SUPABASE_URL;
        var key = AppConfig.SUPABASE_KEY;

        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
        }));
        builder.Services.AddSingleton<ShopView>();
        builder.Services.AddSingleton<ShopViewModel>();
        // Add Data Service
        builder.Services.AddSingleton<IDataService, DataService>();
#if ANDROID
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            if (handler.PlatformView is Android.Widget.EditText nativeEntry)
            {
                nativeEntry.SetBackgroundColor(Android.Graphics.Color.Transparent);
                nativeEntry.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            }
        });
#endif
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}