using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class OtherUserPrifileView : ContentPage
{
    private readonly DataService _dataService;
    private double _panX;
    private AccountViewModel vm;
    private bool _isAnimating;
    private PlantPinModel _pinDataModel;
    public OtherUserPrifileView(PlantPinModel pinDataModel, DataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        _pinDataModel = pinDataModel;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        vm = new AccountViewModel(_dataService);
        BindingContext = vm;
        await vm.Initialize(_pinDataModel);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AccountViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        if (vm != null)
        {
            if (vm.TreesVm != null)
            {
                await vm.TreesVm.RefreshAsync();
            }
        }



        // Підписуємось на скрол
        treesView.Scrolled += TreesView_Scrolled;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is AccountViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        // Відписуємось від скролу
        treesView.Scrolled -= TreesView_Scrolled;
    }

    // ЛОГІКА ПІДНЯТТЯ ТАБІВ ТА ПРИХОВУВАННЯ ПРОФІЛЮ
    private void TreesView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        double maxTranslation = ProfileSection.Height;

        if (maxTranslation <= 0)
            return;

        double currentScroll = e.VerticalOffset;

        // Не даємо елементам піднятися вище, ніж висота блоку профілю
        double translationY = Math.Clamp(currentScroll, 0, maxTranslation);

        // Зсуваємо блоки вгору
        ProfileSection.TranslationY = -translationY;
        ProfileSection.Opacity = 1 - (translationY / maxTranslation); // плавне зникнення профілю

        TabsSection.TranslationY = -translationY;
        ContentSection.TranslationY = -translationY;
    }

    // --- Далі йде ваш старий код свайпів та переключення ---

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