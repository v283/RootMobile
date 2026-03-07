using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using CommunityToolkit.Mvvm.ComponentModel;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class AccountView : ContentPage
{
    private DataService _dataService;
    

    public AccountView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        BindingContext = new AccountViewModel(dataService);
    }
    
}