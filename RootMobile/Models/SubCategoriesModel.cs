using CommunityToolkit.Mvvm.ComponentModel;

namespace RootMobile.Models;

public partial class SubCategoriesModel: ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; }

    [ObservableProperty]
    private bool isSelected;
}