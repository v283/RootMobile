using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class ShopView : ContentPage
{
    private readonly DataService _dataService;
    double screenHeight = 0;

    private System.Timers.Timer _timer;
    ShopViewModel vm;
    
    

    private List<CategoriesModel> categoriesData;
    public ShopView(IDataService dataService)
    {
        InitializeComponent();
        
        screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        prodCollection.MaximumHeightRequest = screenHeight * 1; // Наприклад, 70% висоти екрану
        _dataService = (DataService)dataService;

        Initialize();
        

        // _timer = new System.Timers.Timer(10000); // 10 секунд
        // _timer.Elapsed += (s, e) => AutoScroll();
        // _timer.Start();
    }

    private async void Initialize()
    {
        categoriesData = await _dataService.GetCategories();
        vm = new ShopViewModel(_dataService);
        vm.Initialize(categoriesData);
        BindingContext = vm;
    }
    
    private void SearchBar_TextChanged(object sender, EventArgs e)
    {
        if (BindingContext is ShopViewModel vm)
        {
            vm.FindField = serachBar.Text.Trim();
        }
    }
}