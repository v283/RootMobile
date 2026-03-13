using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

public partial class CommentsView : ContentPage
{
    private readonly CommentsViewModel vm;

    public CommentsView(IDataService _dataService )
    {
        InitializeComponent();
        BindingContext = vm = new CommentsViewModel(_dataService);
        
    }
    public async Task InitializeAsync(PlantPinDataModel plantPost)
    {
        vm.InitializeAsync(plantPost);
    }
}