using CommunityToolkit.Mvvm.ComponentModel;
using RootMobile.Services;

namespace RootMobile.ViewModels;

public partial class BadgesViewModel : ObservableObject
{
    public DataService _dataService;
    public BadgesViewModel(DataService dataService)
    {
        _dataService = dataService;
    }
}