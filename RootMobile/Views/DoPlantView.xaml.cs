using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views;

public partial class DoPlantView : ContentPage
{
    public DoPlantView()
    {
        InitializeComponent();
    }
    

    private async Task ChatBot(Object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"botpage");
    }
}