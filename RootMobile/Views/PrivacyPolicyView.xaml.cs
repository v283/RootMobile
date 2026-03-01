namespace RootMobile.Views;

public partial class PrivacyPolicyView : ContentPage
{
    private const string SupabaseFileUrl = "https://bgrmftzpmzwahkxooxxf.supabase.co/storage/v1/object/public/documents//privacy_policy.html";

    public PrivacyPolicyView()
    {
        InitializeComponent();
        LoadHtmlFromSupabase();
    }

    private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("http"))
        {
            e.Cancel = true; // Скасувати навігацію в WebView
            await Launcher.OpenAsync(e.Url); // Відкрити URL у зовнішньому браузері
        }
    }

    private async void LoadHtmlFromSupabase()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var htmlContent = await httpClient.GetStringAsync(SupabaseFileUrl);

                var htmlSource = new HtmlWebViewSource { Html = htmlContent };
                PrivacyPolicyWebView.Source = htmlSource;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", "Проблема з мережею", "Закрити");
        }
    }
}
