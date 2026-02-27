using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views;

public partial class LaunchView : ContentPage
{
    public LaunchView()
    {
        InitializeComponent();
    }
    private async void OnSignIn(object? sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new SignInView());

    }
    private async void OnSignUp(object? sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new SignUpView());

    }
}