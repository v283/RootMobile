using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;

namespace RootMobile.ViewModels;

public partial class TreesViewModel : ObservableObject
{
    private readonly DataService _dataService;

    private const int ElementsOnPage = 20;
    private int _currentPage = 1;
    private bool _hasMoreItems = true;
    private bool _initialized;

    [ObservableProperty]
    private bool isLoading;
    

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private ObservableRangeCollection<PlantPinDataModel> usersTrees = new();

    public TreesViewModel(DataService dataService)
    {
        _dataService = dataService;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        await RefreshAsync();
    }

    private void ResetAndClear()
    {
        _currentPage = 1;
        _hasMoreItems = true;
        UsersTrees.Clear();
        IsEmpty = false;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsLoading )
            return;

        try
        {
            ResetAndClear();
            await LoadNextPageAsync();
        }
        finally
        {
        }
    }

    [RelayCommand]
    public async Task LoadMoreAsync()
    {
        if (IsLoading || !_hasMoreItems)
            return;

        await LoadNextPageAsync();
    }
    
    [RelayCommand]
    private void ToggleExpand(PlantPinDataModel pin)
    {
        if (pin == null)
            return;

        pin.IsExpanded = !pin.IsExpanded;
    }
    
    private async Task LoadNextPageAsync()
    {
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;

            var trees = await _dataService.GetAllUsersPlantPinsAsync(ElementsOnPage, _currentPage);

            if (trees == null || trees.Count == 0)
            {
                _hasMoreItems = false;
                IsEmpty = UsersTrees.Count == 0;
                return;
            }
            
            UsersTrees.AddRange(trees);

            if (trees.Count < ElementsOnPage)
                _hasMoreItems = false;

            _currentPage++;
            IsEmpty = UsersTrees.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task OpenCommentsAsync(PlantPinDataModel post)
    {
        try
        {
            if (post == null)
                return;
            CommentsView commentsView = new CommentsView(_dataService);
            commentsView.InitializeAsync(post);
            
            await Shell.Current.Navigation.PushAsync(commentsView);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OpenCommentsAsync] Error: {ex.Message}");
        }
    }
}