using CommunityToolkit.Mvvm.ComponentModel;

namespace RootMobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBusy;
}