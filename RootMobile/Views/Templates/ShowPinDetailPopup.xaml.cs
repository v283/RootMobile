using CommunityToolkit.Maui.Views;
using RootMobile.Models;

namespace RootMobile.Views.Templates;

public partial class ShowPinDetailPopup : Popup
{
    public ShowPinDetailPopup(PlantPinModel data)
    {
        InitializeComponent();

        // Заповнюємо дані
        LabelDetailName.Text = data.Name;
        LabelDetailCategory.Text = data.Category;
        LabelDetailComment.Text = string.IsNullOrEmpty(data.Description) ? "No description" : data.Description;
        ImageDetail.Source = ImageSource.FromFile(data.Image);

        // Анімація появи
        Opened += async (s, e) =>
        {
            PopupBorder.TranslationY = 800;
            await PopupBorder.TranslateTo(0, 0, 450, Easing.CubicOut);
        };
    }

    private void OnDetailsScrolled(object sender, ScrolledEventArgs e)
    {
        double fadeDistance = 120;
        double opacity = 1 - (e.ScrollY / fadeDistance);

        if (opacity < 0) opacity = 0;
        if (opacity > 1) opacity = 1;

        ProfileHeader.Opacity = opacity;
        ProfileHeader.IsVisible = opacity > 0.05; // Повністю ховаємо, якщо майже невидимий
    }

    private void OnCloseClicked(object sender, EventArgs e) => Close();
}