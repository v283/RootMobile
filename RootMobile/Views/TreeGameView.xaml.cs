using RootMobile.Models;
using RootMobile.Services;
using System.Diagnostics;

namespace RootMobile.Views;

public partial class TreeGameView : ContentPage
{
    private UserAnimalModel _userAnimalModel;
    private DataService _dataService;
    private Random rng = new();
    private const int SummonCost = 5; // Вартість призову

    public TreeGameView(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCurrentAnimal();
        await RefreshCoins();
    }

    private async Task RefreshCoins()
    {
        var userData = await _dataService.GetUserCoinAmount();
        if (userData != null)
        {
            // Використовуємо MainThread для безпечного оновлення UI
            MainThread.BeginInvokeOnMainThread(() => {
                CoinsLabel.Text = userData.Coins.ToString();
            });
        }
    }
    private async Task LoadCurrentAnimal()
    {
        _userAnimalModel = await _dataService.GetUserCurrentAnimal();
        if (_userAnimalModel?.AnimalType != null)
        {
            TreeSprite.Source = _userAnimalModel.AnimalType.Name + ".png";
        }
    }

    // Відкрити панель призову
    private async void OnOpenSummonPanelClicked(object sender, EventArgs e)
    {
        // 1. Перевірка та списання монет
        bool successSpend = await _dataService.TrySpendCoins(SummonCost);

        if (!successSpend)
        {
            var userData = await _dataService.GetUserCoinAmount();
            await DisplayAlert("Мало 🪙", $"Тобі потрібно {SummonCost} 🪙 для призову!" +
                $"Додай ще рослин на карту і отримай нового героя !:)", "ОК");
            return;
        }

        // Оновлюємо лічильник монет на екрані
        await RefreshCoins();

        // 2. Отримуємо типи персонажів
        var allTypes = await _dataService.GetAllAnimalTypes();
        if (allTypes == null || !allTypes.Any()) return;

        // 3. Рандом
        var randomType = allTypes[rng.Next(allTypes.Count)];

        // 4. Створення нового запису
        var newAnimal = new UserAnimalModel
        {
            AnimalTypeId = randomType.Id,
            Level = 1,
            CurrentHp = randomType.Hp,
            IsActive = false,
            Owner = Guid.Parse(_dataService.GetCurrentUserId())
        };
        Debug.WriteLine("111111111");

        await _dataService.AddNewAnimal(newAnimal);

        // 5. Показ ефектів
        NewCharacterImage.Source = randomType.Name + ".png";
        NewCharacterName.Text = randomType.Name;

        SummonPanel.IsVisible = true;
        await SummonPanel.FadeTo(1, 500);
        await NewCharacterImage.ScaleTo(1.2, 400, Easing.SpringOut);
    }

    private async void OnConfirmSummonClicked(object sender, EventArgs e)
    {
        await SummonPanel.FadeTo(0, 300);
        SummonPanel.IsVisible = false;
        NewCharacterImage.Scale = 0.5;
    }

    private async void OnCharacterTapped(object sender, EventArgs e)
    {
        var allAnimals = await _dataService.GetAllUserAnimals();
        AnimalsList.ItemsSource = allAnimals;

        CharacterSelectionPanel.IsVisible = true;
        await CharacterSelectionPanel.FadeTo(1, 250);
    }

    private async void OnAnimalSelected(object sender, EventArgs e)
    {
        var view = sender as BindableObject;
        var selectedAnimal = view?.BindingContext as UserAnimalModel;

        if (selectedAnimal != null)
        {
            TreeSprite.Source = selectedAnimal.AnimalType.Name + ".png";
            _userAnimalModel = selectedAnimal;

            bool success = await _dataService.UpdateActiveAnimal(selectedAnimal.id);

            if (success)
            {
                await TreeSprite.ScaleTo(1.2, 100);
                await TreeSprite.ScaleTo(1.0, 100);
            }
        }
        ClosePanel();
    }

    private async void ClosePanel()
    {
        if (CharacterSelectionPanel == null) return;
        await CharacterSelectionPanel.FadeTo(0, 200);
        CharacterSelectionPanel.IsVisible = false;
    }

    private async void OnClosePanelClicked(object sender, EventArgs e) => ClosePanel();

    private async void OnStartBattleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1());
    }
}