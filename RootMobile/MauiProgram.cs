using Maui.GoogleMaps.Hosting;
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
using Microsoft.Maui.Platform;
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
        
            PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
    {
        if (handler.PlatformView is MauiPicker nativePicker)
        {
            // remove Material underline/background
            nativePicker.Background = null;
            nativePicker.SetBackgroundColor(Android.Graphics.Color.Transparent);

            // also remove tint that can still draw a line
            nativePicker.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        }
    });
        
#endif
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