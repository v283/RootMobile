using Android.App;
using Android.Runtime;

namespace RootMobile;

[Application]
[MetaData("com.google.android.maps.v2.API_KEY",
            Value = "AIzaSyAKbeCntKNiPUHflmWGV__6ZPT22ppkLdM")]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}