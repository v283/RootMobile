using CommunityToolkit.Mvvm.ComponentModel;

namespace RootMobile.Models;

public partial class RadiusOptionModel : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private int? km;

    [ObservableProperty]
    private bool isSelected;
}