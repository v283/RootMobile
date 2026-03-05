using Microsoft.Extensions.DependencyInjection;
using RootMobile.Services;

namespace RootMobile.Views;

public partial class SignInView : ContentPage
{
    private readonly DataService _dataService;
    private bool _isPasswordVisible;
    private bool _busy;
    
    public SignInView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
    }

    private void SetBusy(bool value)
    {
        _busy = value;

        LoadingIndicator.IsVisible = value;
        LoadingIndicator.IsRunning = value;

        SignInButton.IsEnabled = !value;
        EmailEntry.IsEnabled = !value;
        PasswordEntry.IsEnabled = !value;
        TogglePasswordBtn.IsEnabled = !value;
    }

    private void TogglePasswordBtn_Clicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
    }

    private async void PasswordEntry_Completed(object sender, EventArgs e)
    {
        await TryLoginAsync();
    }

    private async void SignInButton_Clicked(object sender, EventArgs e)
    {
        await TryLoginAsync();
    }

    private async Task TryLoginAsync()
    {
        if (_busy) return;

        var email = EmailEntry.Text?.Trim();
        var pass = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Login", "Enter email.", "OK");
            return;
        }

        if (pass.Length < 6)
        {
            await DisplayAlert("Login", "Password must be at least 6 characters.", "OK");
            return;
        }

        try
        {
            SetBusy(true);

            // як у твоєму LoginPopup
            var ok = await _dataService.LoginAsync(email, pass);

            if (!ok)
            {
                await DisplayAlert("Login", "Wrong email or password.", "OK");
                return;
            }

            // ти відкриваєш SignIn як Modal -> закриваємо його
            if (Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
            else
                await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Login", "Login failed. Try again.", "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void GoToSignUp_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.Navigation.PushModalAsync(new SignUpView(_dataService));
    }

    private async void Google_Tapped(object sender, TappedEventArgs e)
    {
        if (_busy) return;

        try
        {
            SetBusy(true);

            await _dataService.SignInWithGoogleAsync();

            if (Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
            else
                await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Google", "Google sign-in failed.", "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        if (_busy) return;

        var email = await DisplayPromptAsync(
            "Reset password",
            "Enter your email:",
            "Send",
            "Cancel",
            "example@mail.com"
        );

        email = email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            return;

        try
        {
            SetBusy(true);

            var ok = await _dataService.SendPasswordResetEmailAsync(email);
            if (ok)
                await DisplayAlert("Reset password", "Email sent. Check your inbox.", "OK");
            else
                await DisplayAlert("Reset password", "Couldn't send email. Try again.", "OK");
        }
        catch
        {
            await DisplayAlert("Reset password", "Error while sending email.", "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }
}