using CommunityToolkit.Maui.Views;
using RootMobile.Services;
using RootMobile.Models;

namespace RootMobile.Views.Templates;

public partial class FilterPopup : Popup
{
    private readonly DataService _dataService;

    public FilterPopup(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        LoadCategories();
    }

    private async void LoadCategories()
    {
        var cats = await _dataService.GetCategoriesAsync();
        CategoriesList.ItemsSource = cats;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection.FirstOrDefault() as CategoriesMapModel;
        Close(selected?.Name); // Повертаємо назву категорії
    }

    private void OnClearClicked(object sender, EventArgs e) => Close("ALL");
    private void OnCloseClicked(object sender, EventArgs e) => Close(null);
}