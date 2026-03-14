using RootMobile.Models;
using RootMobile.Services;
using RootMobile.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace RootMobile.ViewModels
{
    [QueryProperty(nameof(PromptFromUrl), "prompt")]
    public partial class ConversationViewModel : BaseViewModel
    {
        public string PromptFromUrl
        {
            set
            {
                // Розкодовуємо промпт
                string receivedPrompt = Uri.UnescapeDataString(value ?? "");

                // Запускаємо логіку обробки (не чекаємо на завершення в сеттері)
                _ = ExecuteAiRequest(receivedPrompt);
            }
        }

        private async Task ExecuteAiRequest(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt)) return;

            // Чекаємо півсекунди, щоб сторінка повністю завантажилась і CollectionView був готовий
            await Task.Delay(500);

            // Налаштовуємо режими повідомлення (текст активний)
            TheMessage.IsTextActive = true;
            TheMessage.IsImageActive = false;

            // Встановлюємо текст у поле вводу (Query)
            Query = prompt;

            // Викликаємо ваш існуючий метод AskQuestionAsync
            await AskQuestionAsync();
        }

        [ObservableProperty]
        string query;

        [ObservableProperty]
        bool isAnimationVisible = true;

        [ObservableProperty]
        ObservableCollection<Message> messages = new();

        [ObservableProperty]
        CollectionView collectionView;

        [ObservableProperty]
        ContentPage conversationView;

        [ObservableProperty]
        double opacityModeMessage = 1;

        [ObservableProperty]
        double opacityModeImage = 0.5;

        [ObservableProperty]
        private Message theMessage = new();

        IOpenAIService _openAIService;
        IDispatcher _dispatcher;

        private AsyncRelayCommand _currentCommand;

        public AsyncRelayCommand CurrentCommand
        {
            get
            {
                return _currentCommand ??= new AsyncRelayCommand(AskQuestionAsync);
            }
            set
            {
                SetProperty(ref _currentCommand, value);
            }


        }

        public ConversationViewModel(IOpenAIService openAIService, IDispatcher dispatcher)
        {
            _openAIService = openAIService;
            _dispatcher = dispatcher;
        }

        private void AddMessage(string message, bool isUserMessage)
        {
            if (Messages.Count <= 0) IsAnimationVisible = false;

            Messages.Add(new Message
            {
                Text = message,
                IsUserMessage = isUserMessage,
                IsTextActive = TheMessage.IsTextActive,
                IsImageActive = TheMessage.IsImageActive
            });

            Task.Delay(150).ContinueWith(t =>
            {
                _dispatcher.Dispatch(() =>
                {
                    CollectionView.ScrollTo
                    (
                        item: Messages.Last(),
                        position: ScrollToPosition.End,
                        animate: true
                    );
                });
            });
        }

        private async Task QueryManagerAsync(Func<string, Task<string>> queryManager)
        {
            if (string.IsNullOrEmpty(Query)) return;
            string queryCopy = Query;
            Query = string.Empty;
            AddMessage(message: queryCopy, isUserMessage: true);
            IsBusy = true;
            string answer = await queryManager(queryCopy);
            AddMessage(message: answer.TrimStart(), isUserMessage: false);
            IsBusy = false;
        }

        private async Task AskQuestionAsync()
        {
            TheMessage.IsTextActive = true;
            TheMessage.IsImageActive = false;

            await QueryManagerAsync(_openAIService.AskQuestion);

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // this is for ios in future i'll remove this, when microsoft fixed auto scale, but there will something like animation
                await ConversationView.ScaleTo(0.95, 250, Easing.CubicOut);
                await ConversationView.ScaleTo(1.05, 250, Easing.CubicIn);
                await ConversationView.ScaleTo(1, 250, Easing.CubicOut);
            }

        }



        [RelayCommand]
        private async Task AskQuestion()
        {
            OpacityModeMessage = 1;
            OpacityModeImage = 0.5;

            await Toast.Make("Write mode").Show();

            CurrentCommand = new AsyncRelayCommand(AskQuestionAsync);


        }



        [RelayCommand]
        private async Task SelectTheme()
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;
            AppTheme newTheme = currentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            Application.Current.UserAppTheme = newTheme;

            await ConversationView.ScaleTo(0.95, 250, Easing.CubicOut);
            await ConversationView.ScaleTo(1.05, 250, Easing.CubicIn);
            await ConversationView.ScaleTo(1, 250, Easing.CubicOut);
        }

    }

}
