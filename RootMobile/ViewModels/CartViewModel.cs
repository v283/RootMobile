using System.Reactive.Linq;
using System.Text.Json;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;
using RootMobile.Views.Templates;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace RootMobile.ViewModels
{
	public partial class CartViewModel:ObservableObject 
	{
        private readonly DataService _dataService;

        private int _bottomFilter;

        [ObservableProperty]
        private float totalSum;

        [ObservableProperty]
        private float savedSum;

        [ObservableProperty]
        private float maxSavedSum;

        [ObservableProperty]
        private float maxSum = 0;

        [ObservableProperty]
		public List<SuperCartModel> cartItems;
        [ObservableProperty]
        public List<ProductModel> products;

		public CartViewModel(IDataService dataService)
		{
            _dataService = (DataService)dataService;
        }


        public async Task RefreshCart()
        {
            // if (_dataService.SupabaseClient.Auth.CurrentUser == null)
            // {
            //     await Shell.Current.CurrentPage.ShowPopupAsync(new LoginPopup(_dataService));
            //     await Shell.Current.GoToAsync("//accoutviewpage");
            //     return;
            // }
            //
            // var cartTask = _dataService.GetCartItemAsync();
            // var cartItems = await cartTask;
            //
            // if (cartItems == null || !cartItems.Any())
            // {
            //     CartItems = new List<SuperCartModel>();
            //     Products = new List<ProductModel>();
            //     TotalSum = SavedSum = MaxSavedSum = MaxSum = 0;
            //     return;
            // }
            //
            // var productIds = cartItems.Select(item => item.ProductId).Distinct().ToList();
            // var productsTask = _dataService.GetCart(productIds);
            //
            // var products = await productsTask;
            // var productDict = products.ToDictionary(p => p.Id);
            //
            // foreach (var item in cartItems)
            // {
            //     if (productDict.TryGetValue(item.ProductId, out var product))
            //     {
            //         item.Product = product;
            //         item.Shops = JsonSerializer.Deserialize<List<ShopModel>>(product.Shops);
            //     }
            // }
            //
            // CartItems = cartItems;
            // Products = products;
            //
            // Calculate();
        }


        [RelayCommand]
        private async void ToProductPage(SuperCartModel item)
        {
            ProductView productView = new ProductView(_dataService);
            productView.Initialize(item.Product);
            await Shell.Current.Navigation.PushAsync(productView);
        }

        [RelayCommand]
        private async void DeleteProduct(SuperCartModel item)
        {
            if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                await _dataService.RemoveCartItemAsync(userId, item.Product.Id);
            }
            item.Product.CartIdent = "heart.png";
            await RefreshCart();
        }

        [RelayCommand]
        private async Task ReduceQuantity(SuperCartModel item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity -= 0.1f;
            }
            if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                await _dataService.UpdateCartItemAsync(userId, item.Product.Id, item.Quantity);
            }
            Calculate();
        }

        [RelayCommand]
        private async Task IncreaseQuantity(SuperCartModel item)
        {
            item.Quantity += 0.1f;
            if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                await _dataService.UpdateCartItemAsync(userId, item.Product.Id,item.Quantity);
            }
            Calculate();
        }

        [RelayCommand]
        private async Task UpdateQuantity(SuperCartModel item)
        {
            if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                await _dataService.UpdateCartItemAsync(userId, item.Product.Id, item.Quantity);
            }
            Calculate();
        }


        private  void Calculate()
        {
            // TotalSum = 0;
            // SavedSum = 0;
            // MaxSavedSum = 0;
            //
            // MaxSum = 0;
            // float minSum = 0;
            // foreach (var item in CartItems)
            // {
            //     TotalSum += item.Pay;
            //     float shopMax = item.Shops.OrderBy(s => s.Price).LastOrDefault().Price;
            //     MaxSum += shopMax * item.Quantity;
            //     float shopMin = item.Shops.OrderBy(s => s.Price).FirstOrDefault().Price;
            //     minSum += shopMin * item.Quantity;
            // }
            //
            // SavedSum = MaxSum - TotalSum;
            // MaxSavedSum = MaxSum - minSum;
            //

        }

        [RelayCommand]
        private async Task BottomFilter()
        {
            // var rez = await Shell.Current.CurrentPage.ShowPopupAsync(new CartBottomFilter(_bottomFilter));
            //
            // if (rez != null)
            // {
            //     _bottomFilter = (int)rez;
            // }
            //
            // if (_bottomFilter == 0)
            // {
            //     foreach (var item in CartItems)
            //     {
            //         item.Shop = item.Shops.OrderBy(s => s.Price).FirstOrDefault();
            //     }
            // }
            // else if (_bottomFilter == 1)
            // {
            //     foreach (var item in CartItems)
            //     {
            //         item.Shop = item.Shops.OrderBy(s => s.Price).LastOrDefault();
            //     }
            // }
            // else if (_bottomFilter == 2)
            // {
            //     foreach (var item in CartItems)
            //     {
            //         item.Shop = item.Shops.FirstOrDefault(s => s.LogoUrl == "silpo_logo.png");
            //     }
            // }
            // else if (_bottomFilter == 3)
            // {
            //     foreach (var item in CartItems)
            //     {
            //         item.Shop = item.Shops.FirstOrDefault(s => s.LogoUrl == "fora_logo.png");
            //     }
            // }
            // Calculate();

        }
    }
}

