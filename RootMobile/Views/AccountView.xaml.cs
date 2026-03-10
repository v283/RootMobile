using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class AccountView : ContentPage
{
    private readonly DataService _dataService;
    private double _panX;
    private AccountViewModel vm;
    private bool _isAnimating;

    public AccountView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        InitializeAsync();
    }
    
    private async void InitializeAsync()
    {
        vm = new AccountViewModel(_dataService);
        BindingContext = vm;
        await vm.Initialize();
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (BindingContext is not AccountViewModel viewModel || _isAnimating)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                _panX = e.TotalX;
                break;

            case GestureStatus.Completed:
                if (_panX < -80 && viewModel.Selected != "badges")
                    AnimateSwitch("badges");
                else if (_panX > 80 && viewModel.Selected != "trees")
                    AnimateSwitch("trees");

                _panX = 0;
                break;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AccountViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is AccountViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AccountViewModel.Selected))
        {
            await AnimateTransition(vm.Selected);
        }
    }

    private async Task AnimateTransition(string target)
    {
        if (treesView == null || badgesView == null || _isAnimating)
            return;

        _isAnimating = true;

        bool isTrees = target == "trees";
        VisualElement current = isTrees ? badgesView : treesView;
        VisualElement next = isTrees ? treesView : badgesView;

        if (!current.IsVisible && next.IsVisible)
        {
            _isAnimating = false;
            return;
        }

        double direction = isTrees ? -1 : 1;
        double width = Width > 0 ? Width : Application.Current.MainPage.Width;

        next.TranslationX = direction * width;
        next.Opacity = 0;
        next.IsVisible = true;

        await Task.WhenAll(
            current.TranslateTo(-direction * width, 0, 250, Easing.CubicInOut),
            next.TranslateTo(0, 0, 250, Easing.CubicInOut),
            next.FadeTo(1, 250, Easing.CubicOut)
        );

        current.IsVisible = false;
        current.TranslationX = 0;
        current.Opacity = 1;

        _isAnimating = false;
    }

    private void AnimateSwitch(string tab)
    {
        if (BindingContext is AccountViewModel viewModel)
            viewModel.SelectTabCommand.Execute(tab);
    }
}