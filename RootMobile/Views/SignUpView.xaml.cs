using RootMobile.Services;

namespace RootMobile.Views;

public partial class SignUpView : ContentPage
{
    private readonly DataService _dataService;
    private bool _isPasswordVisible;
    private bool _isConfirmPasswordVisible;
    private bool _busy;

    public SignUpView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;

        HidePasswordBtn.IsVisible = true;
        ViewPasswordBtn.IsVisible = false;

        HideConfirmPasswordBtn.IsVisible = true;
        ViewConfirmPasswordBtn.IsVisible = false;
    }

    private void SetBusy(bool value)
    {
        _busy = value;

        LoadingIndicator.IsVisible = value;
        LoadingIndicator.IsRunning = value;

        SignUpButton.IsEnabled = !value;
        NameEntry.IsEnabled = !value;
        EmailEntry.IsEnabled = !value;
        PasswordEntry.IsEnabled = !value;
        ConfirmPasswordEntry.IsEnabled = !value;

        HidePasswordBtn.IsEnabled = !value;
        ViewPasswordBtn.IsEnabled = !value;
        HideConfirmPasswordBtn.IsEnabled = !value;
        ViewConfirmPasswordBtn.IsEnabled = !value;
    }

    private void TogglePasswordBtn_Tapped(object sender, TappedEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;

        PasswordEntry.IsPassword = !_isPasswordVisible;
        HidePasswordBtn.IsVisible = !_isPasswordVisible;
        ViewPasswordBtn.IsVisible = _isPasswordVisible;
    }

    private void ToggleConfirmPasswordBtn_Tapped(object sender, TappedEventArgs e)
    {
        _isConfirmPasswordVisible = !_isConfirmPasswordVisible;

        ConfirmPasswordEntry.IsPassword = !_isConfirmPasswordVisible;
        HideConfirmPasswordBtn.IsVisible = !_isConfirmPasswordVisible;
        ViewConfirmPasswordBtn.IsVisible = _isConfirmPasswordVisible;
    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        await TrySignUpAsync();
    }

    private async void ConfirmPasswordEntry_Completed(object sender, EventArgs e)
    {
        await TrySignUpAsync();
    }

    private async Task TrySignUpAsync()
    {
        if (_busy)
            return;

        var name = NameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";
        var confirmPassword = ConfirmPasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Sign up", "Enter name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Sign up", "Enter email.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("Sign up", "Password must be at least 6 characters.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Sign up", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            SetBusy(true);

            // Підстав свій реальний метод реєстрації
            var ok = await _dataService.SignUpAsync(email = email, password =password, name = name);

            if (!ok)
            {
                await DisplayAlert("Sign up", "Couldn't create account.", "OK");
                return;
            }

            await DisplayAlert("Sign up", "Account created successfully.", "OK");
            string accessToken = await SecureStorage.Default.GetAsync("authToken");
            string refreshToken = await SecureStorage.Default.GetAsync("refreshToken");
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await Shell.Current.GoToAsync("//rootmapview");
            }
            else
            {


                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();
                else
                    await Navigation.PopAsync();
            }
        }
        catch
        {
            await DisplayAlert("Sign up", "Registration failed. Try again.", "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void Google_Tapped(object sender, TappedEventArgs e)
    {
        if (_busy)
            return;

        try
        {
            SetBusy(true);

            await _dataService.SignInWithGoogleAsync();

            string accessToken = await SecureStorage.Default.GetAsync("authToken");
            string refreshToken = await SecureStorage.Default.GetAsync("refreshToken");
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await Shell.Current.GoToAsync("//rootmapview");
            }
        }
        catch
        {
            await DisplayAlert("Google", "Google sign-up failed.", "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void GoToSignIn_Tapped(object sender, TappedEventArgs e)
    {
        if (Navigation.ModalStack.Count > 0)
            await Navigation.PopModalAsync();
        else
            await Navigation.PopAsync();
    }

    private async void CloseTepped(object sender, TappedEventArgs e)
    {
        if (Navigation.ModalStack.Count > 0)
            await Navigation.PopModalAsync();
        else
            await Navigation.PopAsync();
    }
}