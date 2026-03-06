using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Services;
using RootMobile.Views;

namespace RootMobile.ViewModels;

public partial class SettingsViewModel  : ObservableObject
{
    private DataService _dataService;
    
    public SettingsViewModel(IDataService dataService)
    {
        _dataService = (DataService)dataService;
    }
    
    [RelayCommand]
    private async Task Exit()
    {
        await _dataService.SignOutAsync();
        await Shell.Current.GoToAsync("//rootmapview");
    }
}