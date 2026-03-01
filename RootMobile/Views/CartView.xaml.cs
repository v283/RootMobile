using RootMobile.Services;
using RootMobile.ViewModels;
using Microsoft.Maui.Controls;

namespace RootMobile.Views;

public partial class CartView : ContentPage
{
    private CartViewModel viewModel;
    public CartView(CartViewModel vm)
	{
		InitializeComponent();
		viewModel  = vm;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.RefreshCart();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

}
