using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;
using RootMobile.Views.Templates;

namespace RootMobile.ViewModels
{
    public partial class ShopViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        // Backend filters (2 рівні)
        private string _category = "";     // 1 рівень
        private string _subCategory = "";  // 2 рівень
        private int _bottomFilter;

        // Paging
        private const int ProdOnPage = 54;
        private int _currentPage = 1;
        private bool _isLoading;

        // Search debounce
        private CancellationTokenSource _searchCts;

        // Backup filters when search active
        private List<CategoriesModel> _backupCategories;
        private List<SubCategoriesModel> _backupSubCategories;

        [ObservableProperty]
        private ObservableRangeCollection<ProductModel> products = new();

        // ✅ 1 рівень
        [ObservableProperty]
        private List<CategoriesModel> categories = new();

        // ✅ 2 рівень
        [ObservableProperty]
        private List<SubCategoriesModel> subCategories = new();

        [ObservableProperty]
        private string searchText;

        private CategoriesModel _prevCategorySelected;
        private SubCategoriesModel _prevSubCategorySelected;

        public ShopViewModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task Initialize(List<CategoriesModel> categories)
        {
            Categories = categories ?? new List<CategoriesModel>();
            SubCategories = (Categories.Count > 0 ? Categories[0].SubCategory : new List<SubCategoriesModel>()) ?? new();

            _backupCategories = Categories;
            _backupSubCategories = SubCategories;

            ResetAndClear();
            await LoadCatalog();
        }

        private void ResetAndClear()
        {
            _currentPage = 1;
            Products.Clear();
        }

        private async Task LoadCatalog()
        {
            var products = await _dataService.GetProductsAsync(
                ProdOnPage,
                _currentPage,
                _category,
                _subCategory,
                _bottomFilter,
                SearchText
            );

            if (products is { Count: > 0 })
                Products.AddRange(products);
        }

        // =========================
        // Search (public method)
        // =========================
        public async Task SetSearchAsync(string text)
        {
            // Cancel previous debounce
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(400, _searchCts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            SearchText = text?.Trim();

            // Якщо пошук порожній — повертаємо фільтри назад
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                if (_backupCategories != null) Categories = _backupCategories;
                if (_backupSubCategories != null) SubCategories = _backupSubCategories;
            }
            else
            {
                // Зберігаємо фільтри один раз перед “хованням”
                _backupCategories ??= Categories;
                _backupSubCategories ??= SubCategories;

                // Ховаємо списки фільтрів під час пошуку (як у твоєму старому коді)
                Categories = new();
                SubCategories = new();
            }

            ResetAndClear();
            await LoadCatalog();
        }

        // =========================
        // Paging
        // =========================
        [RelayCommand]
        private async Task LoadMoreProducts()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                _currentPage++;
                await LoadCatalog();
            }
            finally
            {
                _isLoading = false;
            }
        }

        // =========================
        // Filter: Category (level 1)
        // =========================
        [RelayCommand]
        private async Task SelectCategory(CategoriesModel category)
        {
            if (category == null) return;

            // Unselect previous
            if (_prevCategorySelected != null && _prevCategorySelected != category)
                _prevCategorySelected.IsSelected = false;

            // Toggle
            if (!category.IsSelected)
            {
                category.IsSelected = true;
                _prevCategorySelected = category;

                _category = category.Name ?? "";
                _subCategory = "";

                SubCategories = category.SubCategory ?? new List<SubCategoriesModel>();

                // Зняти виділення підкатегорії
                if (_prevSubCategorySelected != null)
                {
                    _prevSubCategorySelected.IsSelected = false;
                    _prevSubCategorySelected = null;
                }
            }
            else
            {
                category.IsSelected = false;
                _prevCategorySelected = null;

                _category = "";
                _subCategory = "";

                SubCategories = new List<SubCategoriesModel>();

                if (_prevSubCategorySelected != null)
                {
                    _prevSubCategorySelected.IsSelected = false;
                    _prevSubCategorySelected = null;
                }
            }

            ResetAndClear();
            await LoadCatalog();
        }

        // =========================
        // Filter: SubCategory (level 2)
        // =========================
        [RelayCommand]
        private async Task SelectSubCategory(SubCategoriesModel sub)
        {
            if (sub == null) return;

            if (_prevSubCategorySelected != null && _prevSubCategorySelected != sub)
                _prevSubCategorySelected.IsSelected = false;

            if (!sub.IsSelected)
            {
                sub.IsSelected = true;
                _prevSubCategorySelected = sub;

                _subCategory = sub.Name ?? "";
            }
            else
            {
                sub.IsSelected = false;
                _prevSubCategorySelected = null;

                _subCategory = "";
            }

            ResetAndClear();
            await LoadCatalog();
        }

        // =========================
        // Navigation
        // =========================
        [RelayCommand]
        private async Task ToProductPage(ProductModel product)
        {
            if (product == null) return;

            var productView = new ProductView(_dataService);
            productView.Initialize(product);
            await Shell.Current.Navigation.PushAsync(productView);
        }

        // =========================
        // Cart
        // =========================
        [RelayCommand]
        private async Task AddOrRemoveToCart(ProductModel product)
        {
            if (product == null) return;

            if (_dataService.SupabaseClient.Auth.CurrentUser == null)
            {
                await Shell.Current.Navigation.PushModalAsync(new SignInView(_dataService));
                return;
            }

            if (!Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
                return;

            if (product.CartIdent == "heart.png")
            {
                await _dataService.AddCartItemAsync(userId, product.Id, product.TableName, 1);
                product.CartIdent = "heart_done.png";
            }
            else
            {
                await _dataService.RemoveCartItemAsync(userId, product.Id);
                product.CartIdent = "heart.png";
            }
        }

        // =========================
        // Bottom filter (sort)
        // =========================
        [RelayCommand]
        private async Task BottomFilter()
        {
            var rez = await Shell.Current.CurrentPage.ShowPopupAsync(new BottomSheetFilter(_bottomFilter));
            if (rez != null)
                _bottomFilter = (int)rez;

            ResetAndClear();
            await LoadCatalog();
        }
    }
}