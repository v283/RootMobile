using RootMobile.Services;
using RootMobile.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace RootMobile;
public partial class App : Application
{
        public static DataService HotDataService;
    public readonly DataService _dataService;

    public App(IDataService dataService)
    {
        
        _dataService = (DataService)dataService;
        HotDataService = _dataService;
        InitializeComponent();

        InitializeAccountAsync();

        WeakReferenceMessenger.Default.Register<NotificationItemMessage>(this, (r, m) =>
        {
            string gettoken = m.Value.ToString();
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                GetApiCalls(gettoken);


                return false;
            });
        });

    }




    private async void GetApiCalls(string id)
    {

        try
        {

            var product = await _dataService.GetProductByIdAsync(Convert.ToInt32(id));
            ProductView productView = new ProductView(_dataService);
            productView.Initialize(product);
            await Shell.Current.Navigation.PushAsync(productView);
        }
        catch (Exception ex)
        {
            await AppShell.Current.DisplayAlert("Посилання не доступне", id.ToString(), "ok");
        }


    }


    private async Task InitializeAccountAsync()
    {
        try
        {
            bool isRestored = await _dataService.RestoreSessionAsync();

            if (_dataService.SupabaseClient.Auth.CurrentUser != null)
            {
                await _dataService.Initialize();
            }
        }
        catch (Exception ex)
        {
            await _dataService.SignOutAsync();
        }


    }



    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);

        if (uri.Scheme == "https"
            && uri.Host == "v283.github.io"
            && uri.AbsolutePath.StartsWith("/avocado-site/product"))
        {
            // приклад: /avocado-site/product/token=23643
            var lastSegment = uri.AbsolutePath.Split("token=").Last();

            if (!string.IsNullOrEmpty(lastSegment))
            {
                await Shell.Current.GoToAsync($"product/{lastSegment}");
            }
        }
    }

    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}