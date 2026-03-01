using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;

namespace RootMobile.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class ProductView : ContentPage
{
    private string itemId;
    public string ItemId
    {
        get => itemId;
        set
        {
            itemId = value;
            if (int.TryParse(itemId, out int id))
            {
                LoadProductAsync(id);
            }
        }
    }

    private readonly IDataService _dataService;
    public ProductView(IDataService dataService)
    {
        _dataService = dataService;
        InitializeComponent();
    }
    


    public void Initialize(ProductModel product)
    {
        ProductViewModel vm = new ProductViewModel(_dataService);
        vm.Initialize(product);

        BindingContext = vm;
    }

    async void LoadProductAsync(int id)
    {


        try
        {
            var response = await ((DataService)_dataService)
                .SupabaseClient
                .From<ProductModel>()
                .Where(x => x.Id == id)
                .Get();

            var product = response.Models.FirstOrDefault();

            if (product != null)
                Initialize(product);
            else
                await DisplayAlert("Помилка", $"Товар з id={id} не знайдено", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Критична помилка", ex.Message, "OK");
        }
    }


}
