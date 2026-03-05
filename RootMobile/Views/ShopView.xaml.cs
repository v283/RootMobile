using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class ShopView : ContentPage
{
    private readonly DataService _dataService;
    private ShopViewModel vm;

    public ShopView(IDataService dataService)
    {
        InitializeComponent();

        _dataService = (DataService)dataService;
        Initialize();
    }

    private async void Initialize()
    {
        var categoriesData = await _dataService.GetCategories();

        vm = new ShopViewModel(_dataService);
        BindingContext = vm;

        await vm.Initialize(categoriesData);
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is ShopViewModel viewModel)
            await viewModel.SetSearchAsync(e.NewTextValue);
    }
}