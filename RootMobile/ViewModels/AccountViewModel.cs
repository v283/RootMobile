using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;

namespace RootMobile.ViewModels;

public partial class AccountViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private UserDataModel userData;

    [ObservableProperty]
    private string selected = "trees";

    public bool IsTreesSelected => Selected == "trees";
    public bool IsBadgesSelected => Selected == "badges";

    [ObservableProperty]
    private TreesViewModel treesVm;

    [ObservableProperty]
    private BadgesViewModel badgesVm;

    private PlantPinModel _pinDataMode;
    

    public AccountViewModel(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        
    }

    partial void OnSelectedChanged(string value)
    {
        OnPropertyChanged(nameof(IsTreesSelected));
        OnPropertyChanged(nameof(IsBadgesSelected));
    }

    [RelayCommand]
    private void SelectTab(string tab)
    {
        string newTab = tab.ToLower();
        if (Selected != newTab)
        {
            Selected = newTab;
        }
    }

    public async Task Initialize(PlantPinModel pinDataModel = null)
    {
        _pinDataMode = pinDataModel;
        if (pinDataModel == null)
        {
            UserData = await _dataService.GetUserData();
        }
        else
        {
            UserData = await _dataService.GetUserData(_pinDataMode.UserId);
        }

        TreesVm = new TreesViewModel(_dataService);
        BadgesVm = new BadgesViewModel(_dataService);
        if (_pinDataMode != null)
        {
            await TreesVm.InitializeAsync(_pinDataMode.UserId);
            await BadgesVm.Initialize();
        }

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