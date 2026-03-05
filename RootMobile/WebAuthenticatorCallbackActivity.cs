#if ANDROID

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using Microsoft.Maui.Authentication;
using Microsoft.Maui.ApplicationModel;
namespace RootMobile
{

    [Activity(
        Name = "RootMobile.WebAuthenticatorCallbackActivity",
        Exported = true,
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.valentineos.rootmobile",
        DataHost = "callback")]

    public class WebAuthenticatorCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Handle the OAuth callback
            Microsoft.Maui.ApplicationModel.Platform.OnNewIntent(Intent); // Use Microsoft.Maui.ApplicationModel
        }
    }
}
#endif