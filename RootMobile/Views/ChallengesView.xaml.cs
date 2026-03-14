using System.Collections.ObjectModel;
using RootMobile.Services;
using RootMobile.ViewModels; // Перемісти модель сюди

namespace RootMobile.Views;

public partial class ChallengesView : ContentPage
{
    public ObservableCollection<ChallengeItemModel> Challenges { get; set; } = new();
    private readonly DataService _dataService;

    public ChallengesView(IDataService dataService)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;

        LoadMockData();
        BindingContext = this;
    }

    private void LoadMockData()
    {
        Challenges.Add(new ChallengeItemModel
        {
            Id = 1,
            Title = "Перші кроки",
            Description = "Відкрий цей розділ та ознайомся зі списком справ.",
            RewardCoins = 10,
            IsCompleted = false
        });
        
        Challenges.Add(new ChallengeItemModel
        {
            Id = 2,
            Title = "Еко-активіст",
            Description = "Висади своє перше віртуальне дерево у грі.",
            RewardCoins = 50,
            IsCompleted = true
        });
    }

    private async void OnChallengeClicked(object sender, EventArgs e)
    {
        if (sender is Button { BindingContext: ChallengeItemModel challenge })
        {
            // Тут ти пізніше додаси логіку через Supabase

            await _dataService.AddCoins(challenge.RewardCoins);
            Challenges.Where(x=> x.Id == challenge.Id).First().IsCompleted = true;
            await DisplayAlert(
                challenge.Title,
                $"Нагорода: {challenge.RewardCoins} монеток.\nСтатус: {(challenge.IsCompleted ? "Завершено" : "В процесі")}",
                "Зрозуміло");
        }
    }
}

public class ChallengeItemModel : System.ComponentModel.INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int RewardCoins { get; set; }
    
    private bool _isCompleted;
    public bool IsCompleted 
    { 
        get => _isCompleted; 
        set { _isCompleted = value; OnPropertyChanged(nameof(IsCompleted)); } 
    }

    public string RewardText => $"+{RewardCoins}";

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}