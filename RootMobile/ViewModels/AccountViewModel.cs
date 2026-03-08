using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;

namespace RootMobile.ViewModels;

public partial class AccountViewModel  : ObservableObject
{
    private DataService _dataService;

    [ObservableProperty] 
    private UserDataModel userData;
    
    public AccountViewModel(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        Initialize();

    }

    private async Task Initialize()
    {
        UserData =  await _dataService.GetUserData();
    }

    [RelayCommand]
    private async Task GoToSettings()
    {
        await Shell.Current.Navigation.PushAsync(new SettingsView(_dataService));
    }
    
    [RelayCommand]
    private async Task GoToEditAccount()
    {
        await Shell.Current.Navigation.PushAsync(new EditAccountView(_dataService, UserData));
    }
    
    [RelayCommand]
    private async Task GoToEmails()
    {
        await Shell.Current.Navigation.PushAsync(new EmailsView());
    }
    
    
}