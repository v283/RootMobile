using CommunityToolkit.Mvvm.ComponentModel;
using RootMobile.Models;
using RootMobile.Services;

namespace RootMobile.ViewModels;

public partial class BadgesViewModel : ObservableObject
{
    public DataService _dataService;

    [ObservableProperty]
    private List<BadgeModel> badgesCollection;
    
    public BadgesViewModel(DataService dataService)
    {
        _dataService = dataService;
        
    }

    public async Task Initialize()
    {
        BadgesCollection = await _dataService.GetBadges();
    }
}