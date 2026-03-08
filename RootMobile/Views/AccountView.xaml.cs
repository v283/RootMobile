using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class AccountView : ContentPage
{
    private readonly DataService _dataService;
    private double _panX;

    public AccountView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        BindingContext = new AccountViewModel(dataService);
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (BindingContext is not AccountViewModel vm)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                _panX = e.TotalX;
                break;

            case GestureStatus.Completed:
                if (_panX < -80)
                    AnimateSwitch("badges");
                else if (_panX > 80)
                    AnimateSwitch("trees");

                _panX = 0;
                break;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AccountViewModel vm)
        {
            vm.PropertyChanged += async (_, e) =>
            {
                if (e.PropertyName == nameof(vm.Selected))
                    await AnimateTransition(vm.Selected);
            };
        }
    }

    private async Task AnimateTransition(string target)
    {
        if (treesView == null || badgesView == null)
            return;

        bool isTrees = target == "trees";
        VisualElement current = isTrees ? badgesView : treesView;
        VisualElement next = isTrees ? treesView : badgesView;

        double direction = isTrees ? -1 : 1;

        next.TranslationX = direction * Width;
        next.Opacity = 0;
        next.IsVisible = true;

        await Task.WhenAll(
            current.TranslateTo(-direction * Width, 0, 250, Easing.CubicInOut),
            next.TranslateTo(0, 0, 250, Easing.CubicInOut),
            next.FadeTo(1, 250, Easing.CubicOut)
        );

        current.IsVisible = false;
        current.TranslationX = 0;
        current.Opacity = 1;
    }

    private void AnimateSwitch(string tab)
    {
        if (BindingContext is AccountViewModel vm)
            vm.SelectTabCommand.Execute(tab);
    }
}