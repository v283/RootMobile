using RootMobile.Models;
using RootMobile.Services;

namespace RootMobile.Views;

public partial class TreeGameView : ContentPage
{
    private UserAnimalModel _userAnimalModel;
    private DataService _dataService;
    private Random rng = new();
    public TreeGameView(IDataService dataService)
    {
        _dataService = (DataService)dataService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCurrentAnimal();
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
        // 1. Отримуємо список доступних типів тварин (щоб знати кого призивати)
        var allTypes = await _dataService.GetAllAnimalTypes();
        if (allTypes == null || !allTypes.Any()) return;

        // 2. Рандомимо персонажа
        var randomType = allTypes[rng.Next(allTypes.Count)];

        // 3. Готуємо модель для вставки
        var newAnimal = new UserAnimalModel
        {
            AnimalTypeId = randomType.Id,
            Level = 1,
            CurrentHp = randomType.Hp,
            IsActive = false, // Новий герой не стає активним автоматично
            Owner = Guid.Parse(_dataService.GetCurrentUserId()) // Метод отримання ID з Supabase
        };

        // 4. Зберігаємо в базу
        await _dataService.AddNewAnimal(newAnimal);

        // 5. Показуємо візуальний ефект
        NewCharacterImage.Source = randomType.Name + ".png";
        NewCharacterName.Text = randomType.Name;

        SummonPanel.IsVisible = true;
        await SummonPanel.FadeTo(1, 500);

        // Анімація "вистрибування" персонажа
        await NewCharacterImage.ScaleTo(1.2, 400, Easing.SpringOut);
    }

    private async void OnConfirmSummonClicked(object sender, EventArgs e)
    {
        await SummonPanel.FadeTo(0, 300);
        SummonPanel.IsVisible = false;
        NewCharacterImage.Scale = 0.5; // Скидаємо для наступного разу
    }
    // Відкриваємо панель при натисканні на персонажа
    private async void OnCharacterTapped(object sender, EventArgs e)
    {
        // Отримуємо всіх тварин користувача (активних і ні)
        var allAnimals = await _dataService.GetAllUserAnimals();
        AnimalsList.ItemsSource = allAnimals;

        CharacterSelectionPanel.IsVisible = true;
        await CharacterSelectionPanel.FadeTo(1, 250);
    }

    // Логіка вибору нового персонажа
    private async void OnAnimalSelected(object sender, EventArgs e)
    {
        var view = sender as BindableObject;
        var selectedAnimal = view?.BindingContext as UserAnimalModel;

        if (selectedAnimal != null)
        {
            // 1. Візуальне оновлення (щоб юзер не чекав відповіді БД)
            TreeSprite.Source = selectedAnimal.AnimalType.Name + ".png";
            _userAnimalModel = selectedAnimal;

            // 2. Оновлення в БД
            bool success = await _dataService.UpdateActiveAnimal(selectedAnimal.id);

            if (success)
            {
                // Анімація успішного вибору
                await TreeSprite.ScaleTo(1.2, 100);
                await TreeSprite.ScaleTo(1.0, 100);
            }
            else
            {
                // Якщо сталася помилка (наприклад, немає інтернету)
                await DisplayAlert("Помилка", "не вдалося зберегти вибір у хмарі", "ОК");
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
    private async void OnClosePanelClicked(object sender, EventArgs e)
    {
        await CharacterSelectionPanel.FadeTo(0, 200);
        CharacterSelectionPanel.IsVisible = false;
    }

    private async void OnStartBattleClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1());
    }
}