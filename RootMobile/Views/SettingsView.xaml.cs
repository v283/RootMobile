using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class SettingsView : ContentPage
{
    public SettingsView(IDataService dataService)
    {
        InitializeComponent();
        BindingContext = new SettingsViewModel(dataService);

    }
}