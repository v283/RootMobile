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
    private TreesViewModel treesVm = new();

    [ObservableProperty]
    private BadgesViewModel badgesVm = new();

    public AccountViewModel(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        _ = Initialize();
    }

    partial void OnSelectedChanged(string value)
    {
        OnPropertyChanged(nameof(IsTreesSelected));
        OnPropertyChanged(nameof(IsBadgesSelected));
    }

    [RelayCommand]
    private void SelectTab(string tab)
    {
        Selected = tab.ToLower();

        if (tab == "trees")
        {
            TreesVm = new TreesViewModel(_dataService);
        }
        else if (tab == "badges")
        {
            BadgesVm = new BadgesViewModel(_dataService);
        }
            

    }

    [RelayCommand]
    private void Swipe(string direction)
    {
        if (direction == "left" && Selected == "trees")
            Selected = "badges";
        else if (direction == "right" && Selected == "badges")
            Selected = "trees";
    }

    private async Task Initialize()
    {
        UserData = await _dataService.GetUserData();
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