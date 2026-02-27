using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views;

public partial class RootMapView : ContentPage
{
    private bool isSignIn =  false;
    public RootMapView()
    {
        InitializeComponent();
        if (!isSignIn)
        {
            Shell.Current.Navigation.PushModalAsync(new LaunchView(), true);
        }
        
    }
}