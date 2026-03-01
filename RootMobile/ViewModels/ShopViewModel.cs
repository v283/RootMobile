using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;
using RootMobile.Views.Templates;


namespace RootMobile.ViewModels
{
    public partial class ShopViewModel : ObservableObject
    {

        private readonly DataService _dataService;

        private string _category;
        private string _sub;
        private string _subSub;
        private int _bottomFilter;

        private string findField;

        public string FindField
        {
            get => findField;
            set
            {
                _currentPage = 0;
                if (backSubFilter == null || backSubSubFilter == null)
                {
                    backSubFilter = SubFilter;
                    backSubSubFilter = SubSubFilter;
                }
                if (string.IsNullOrEmpty(value))
                {
                    SubFilter = backSubFilter;
                    SubSubFilter = backSubSubFilter;
                }
                else
                {

                    findField = value;
                    ProductsBind.Clear();;
                    LoadCatalog();//.Wait();

                    if (SubFilter != null && SubSubFilter!= null)
                    {
                        if (SubFilter.Count > 0 || SubSubFilter.Count > 0)
                        {
                            SubFilter = new();
                            SubSubFilter = new();
                        }
                    }

                }

            }
        }

        private int _prodOnPage = 54;
        private int _currentPage = 1;
        private bool _isLoading = false;

        [ObservableProperty]
        public ObservableRangeCollection<ProductModel> productsBind;

        private List<CategoriesModel> backSubFilter;
        [ObservableProperty]
        public List<CategoriesModel> subFilter;

        private List<SubCategoriesModel> backSubSubFilter;
        [ObservableProperty]
        public List<SubCategoriesModel> subSubFilter;

        public ShopViewModel(DataService dataService)
        {
            _dataService = dataService;
            ProductsBind = new();
        }

        public async Task Initialize(string category, CategoriesModel sub, List<SubCategoriesModel> subList, string serchFilter="",string subStr="", string subsub="")
        {
            _category = category;
            _sub = sub.Name;
            SubSubFilter = sub.SubCategory;
            //SubFilter = subList;

            _subSub = subsub;
            if (!subStr.IsNullOrEmpty())
            {
                _sub = subStr;
            }
            if (!string.IsNullOrEmpty(serchFilter))
            {
                FindField = serchFilter;
            }
            else { await LoadCatalog(); }
            

        }

        private async Task LoadCatalog()
        {
            var products = await _dataService.GetProductsAsync(_prodOnPage, _currentPage, _category, _sub,_subSub, _bottomFilter, findField);
            ProductsBind.AddRange(products);
            // if (products.Count > 0)
            // {
            //     foreach (var product in products)
            //     {
            //         ProductsBind.Add(product);
            //     }  
            // }
        }

        [RelayCommand]
        private async Task LoadMoreProducts()
        {
            if (_isLoading) return;
            _isLoading = true;
            _currentPage++;
            await LoadCatalog();
            _isLoading = false;
        }

        private SubCategoriesModel prevSubSub = new();
        [RelayCommand]
        private async Task FilterSubSub(SubCategoriesModel subSub)
        {
            if (prevSubSub != null && prevSubSub != subSub)
            {
                prevSubSub.IsSelected = false;
            }

            if (!subSub.IsSelected)
            {
                subSub.IsSelected = true;
                _subSub = subSub.Name;
                prevSubSub = subSub;
            }
            else
            {
                subSub.IsSelected = false;
                _subSub = "";
                prevSubSub = new(); ;
            }

            _currentPage = 0;
            ProductsBind.Clear();;
            await LoadCatalog();
        }

        private CategoriesModel prevSub = new();
        [RelayCommand]
        private async Task FilterSub(CategoriesModel sub)
        {
            if (prevSub != null && prevSub != sub)
            {
                prevSub.IsSelected = false;
            }

            if (!sub.IsSelected)
            {
                sub.IsSelected = true;
                _sub = sub.Name;
                SubSubFilter = sub.SubCategory;
                _subSub = "";
                prevSub = sub;
            }
            else
            {
                sub.IsSelected = false;
                _sub = "";
                SubSubFilter = new();
                _subSub = "";
                prevSub = new();
            }

            _currentPage = 0;
            ProductsBind.Clear();;
            await LoadCatalog();
        }

        [RelayCommand]
        private async Task ToProductPage(ProductModel product)
        { 
            ProductView productView = new ProductView(_dataService);
            productView.Initialize(product);
            await Shell.Current.Navigation.PushAsync(productView);
            
            //AppShell.AddCountToShow();
        }

        [RelayCommand]
        private async Task AddOrRemoveToCart(ProductModel product)
        {
            // if (_dataService.SupabaseClient.Auth.CurrentUser != null)
            // {
                if (product.CartIdent == "heart.png")
                {
                    if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
                    {
                        await _dataService.AddCartItemAsync(userId, product.Id, product.TableName, 1);
                    }
                    product.CartIdent = "heart_done.png";
                }
                else
                {
                    if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
                    {
                        await _dataService.RemoveCartItemAsync(userId, product.Id);
                    }
                    product.CartIdent = "heart.png";
                }
            // }
            // else
            // {
            //     await Shell.Current.CurrentPage.ShowPopupAsync(new LoginPopup(_dataService));
            //
            // }

        }

        [RelayCommand]
        private async Task BottomFilter()
        {
            var rez = await Shell.Current.CurrentPage.ShowPopupAsync(new BottomSheetFilter(_bottomFilter));

            if(  rez!= null )
            {
                _bottomFilter = (int)rez;
            }
            _currentPage = 0;
            //ProductsBind = new();
            ProductsBind.Clear();
            await LoadCatalog();
        }
        
    }
}
