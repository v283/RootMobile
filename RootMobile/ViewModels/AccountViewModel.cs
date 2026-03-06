using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Services;
using RootMobile.Views;

namespace RootMobile.ViewModels;

public partial class AccountViewModel  : ObservableObject
{
    private DataService _dataService;
    
    public AccountViewModel(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        
    }

    [RelayCommand]
    private async Task GoToSettings()
    {
        await Shell.Current.Navigation.PushAsync(new SettingsView(_dataService));
    }
    
    [RelayCommand]
    private async Task GoToEditAccount()
    {
        await Shell.Current.Navigation.PushAsync(new EditAccountView());
    }
    
    [RelayCommand]
    private async Task GoToEmails()
    {
        await Shell.Current.Navigation.PushAsync(new EmailsView());
    }
    
    
}