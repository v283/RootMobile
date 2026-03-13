using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class CommunityPostsView : ContentPage
{
    private CommunityPostsViewModel _vm;
    private IDataService _dataService;
    
    public CommunityPostsView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var _vm = new CommunityPostsViewModel((DataService)_dataService);
        await _vm.InitializeAsync();
        BindingContext = _vm;
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _vm.SetSearchAsync(e.NewTextValue);
    }
}