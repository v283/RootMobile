using CommunityToolkit.Maui.Views;
using RootMobile.Services;
using RootMobile.Models;
using System.Collections.ObjectModel;

namespace RootMobile.Views.Templates;

public partial class FilterPopup : Popup
{
    private readonly DataService _dataService;
    public ObservableCollection<CategoriesMapModel> Categories { get; set; } = new();

    public FilterPopup(IDataService dataService, List<string> selectedNames = null)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;

        // Встановлюємо ItemsSource для BindableLayout
        BindableLayout.SetItemsSource(CategoriesFlex, Categories);

        LoadCategories(selectedNames);
    }

    private async void LoadCategories(List<string> selectedNames)
    {
        var cats = await _dataService.GetCategoriesAsync();
        if (cats == null) return;

        foreach (var cat in cats)
        {
            // Якщо назва категорії є у списку вже обраних — ставимо IsSelected = true
            if (selectedNames != null && selectedNames.Contains(cat.Name))
                cat.IsSelected = true;
            else
                cat.IsSelected = false;

            Categories.Add(cat);
        }
    }

    private void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CategoriesMapModel tappedCategory)
        {
            // Перемикаємо стан
            tappedCategory.IsSelected = !tappedCategory.IsSelected;
        }
    }

    private void OnApplyClicked(object sender, EventArgs e)
    {
        // Збираємо назви всіх категорій, де IsSelected == true
        var result = Categories.Where(x => x.IsSelected).Select(x => x.Name).ToList();
        Close(result);
    }

    private void OnCloseClicked(object sender, EventArgs e) => Close(null);
}