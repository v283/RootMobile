using CommunityToolkit.Mvvm.ComponentModel;
using RootMobile.Services;

namespace RootMobile.ViewModels;

public partial class TreesViewModel : ObservableObject
{
    public DataService _dataService;
    public TreesViewModel(DataService dataService)
    {
        _dataService = dataService;
    }
}