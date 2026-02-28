using CommunityToolkit.Mvvm.ComponentModel;

namespace RootMobile.Models;

public partial class CategoriesModel: ObservableObject
{
    public string Name { get; set; }
    public List<SubCategoriesModel> SubCategory { get; set; }

    [ObservableProperty]
    private bool isSelected;
}