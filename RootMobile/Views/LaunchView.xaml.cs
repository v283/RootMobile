using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Services;

namespace RootMobile.Views;

public partial class LaunchView : ContentPage
{
    private IDataService _dataService;
    public LaunchView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
    }
    private async void OnSignIn(object? sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new SignInView(_dataService));

    }
    private async void OnSignUp(object? sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new SignUpView(_dataService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        string accessToken = await SecureStorage.Default.GetAsync("authToken");
        string refreshToken = await SecureStorage.Default.GetAsync("refreshToken");
        if (!string.IsNullOrEmpty(accessToken) &&
            !string.IsNullOrEmpty(refreshToken))
        {
            await Shell.Current.GoToAsync("//rootmapview");
        }
    }
}