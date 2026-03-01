using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;

using System.Text.Json;
using RootMobile.Services;
using CommunityToolkit.Maui.Views;
//using RootMobile.Views.Templates;
using RootMobile.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Extensions;

namespace RootMobile.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private ProductModel product;
        

        public ProductViewModel(IDataService dataService)
        {
            _dataService = (DataService)dataService;

        }

        public void Initialize(ProductModel p)
        {
            product = p;
        }



        [RelayCommand]
        private async void GoToShop(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occurred. No browser may be installed on the device.
            }
        }
        [RelayCommand]
        private async void AddOrRemoveToCart()
        {
            // if (_dataService.SupabaseClient.Auth.CurrentUser != null)
            // {
                if (Product.CartIdent == "heart.png")
                {
                    if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
                    {
                        await _dataService.AddCartItemAsync(userId, Product.Id, Product.TableName, 1);
                    }
                    Product.CartIdent = "heart_done.png";
                }
                else
                {
                    if (Guid.TryParse(_dataService.SupabaseClient.Auth.CurrentUser.Id, out var userId))
                    {
                        await _dataService.RemoveCartItemAsync(userId, Product.Id);
                    }
                    Product.CartIdent = "heart.png";
                }
            // }
            // else { await Shell.Current.CurrentPage.ShowPopupAsync(new LoginPopup(_dataService)); }
            //
            //

        }

        [RelayCommand]
        private async Task ChatBot()
        {
            string temp = $"Скільки калорій в {product.Size} {product.Name}?";
            await Shell.Current.GoToAsync($"botpage?question={temp}");
        }

        [RelayCommand]
        private async Task ShareProduct()
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = "https://v283.github.io/avocado-site/product/token="+product.Id,
                Title = "Поділитись з друзями"
            });
        }


        [RelayCommand]
        private async Task Mark()
        {

            //await Shell.Current.Navigation.PushAsync(new MarkView(_dataService,product));
        }
    }
}

