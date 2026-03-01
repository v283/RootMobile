using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
//using Plugin.MauiMTAdmob;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;

namespace RootMobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] {
        Intent.ActionView,
        Intent.CategoryDefault,
        Intent.CategoryBrowsable
    },
    DataScheme = "http",
    DataHost = "v283.github.io",
    DataPathPrefix = "/product"
)]

[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] {
        Intent.ActionView,
        Intent.CategoryDefault,
        Intent.CategoryBrowsable
    },
    DataScheme = "https",
    DataHost = "v283.github.io",
    DataPathPrefix = "/product"
)]
public class MainActivity : MauiAppCompatActivity
{
        protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set status bar color to white
        Window.SetStatusBarColor(Android.Graphics.Color.White);
        // Set dark icons for status bar
        Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;

        // Adjust keyboard for AI chat
        Window.SetSoftInputMode(SoftInput.AdjustResize);



        Platform.OnNewIntent(Intent);

        //CrossMauiMTAdmob.Current.Init( this, "ca-app-pub-2108598265596673~7672224429"); // change to real


        var uri = Intent?.Data;

        if (uri != null)
        {
            // Наприклад: https://v283.github.io/avocado-site/product/token=23643

            string path = uri.Path; // Android.Net.Uri.Path
            string query = uri.EncodedQuery; // token=23643

            string token = null;

            if (!string.IsNullOrEmpty(query) && query.StartsWith("token="))
            {
                token = query.Replace("token=", "");
            }
            else if (path.Contains("token="))
            {
                token = path.Split("token=").Last();
            }

            if (!string.IsNullOrEmpty(token))
                WeakReferenceMessenger.Default.Send(new NotificationItemMessage(token));
        }


    }


    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        var action = intent.Action;
        var strLink = intent.DataString;
        if (Intent.ActionView == action && !string.IsNullOrWhiteSpace(strLink))
        {
            //this get's triggered when the link clicked for the 2nd, 3rd or any infinity time and app is open in backgraound

            Toast.MakeText(this, strLink, ToastLength.Short).Show();
            //Extract out token from here:
            string apitoken = strLink.Substring(strLink.LastIndexOf('=') + 1);




            WeakReferenceMessenger.Default.Send(new NotificationItemMessage(apitoken));
        };
    }

}