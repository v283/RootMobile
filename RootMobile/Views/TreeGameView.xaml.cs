using RootMobile.Models;
using RootMobile.Services;
using System.Diagnostics;
namespace RootMobile.Views;
public partial class TreeGameView : ContentPage
{
    private UserAnimalModel _userAnimalModel;
    private AnimalTypeModel _animalTypeModel;

    private DataService _dataService;

    public TreeGameView(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        Debug.WriteLine("11111111111");

        base.OnAppearing();
        Debug.WriteLine("11111111111");

        _userAnimalModel = await _dataService.GetUserCurrentAnimal();

        if (_userAnimalModel?.AnimalType != null)
        {
            _animalTypeModel =await _dataService.GetAnimalType(_userAnimalModel.AnimalTypeId) ;
            TreeSprite.Source = _animalTypeModel.Name + ".png";
            Console.WriteLine(_animalTypeModel.Name + ".png");
            Debug.WriteLine("11111111111");
            Debug.WriteLine(_animalTypeModel.Name + ".png");
        }
    }
}