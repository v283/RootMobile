using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;
using RootMobile.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RootMobile.ViewModels;

public partial class CommunityPostsViewModel : ObservableObject
{
    private readonly DataService _dataService;

    private const int ElementsOnPage = 20;
    private int _currentPage = 1;
    private bool _hasMoreItems = true;
    private bool _initialized;
    private bool _isLoadingMore;
    private CancellationTokenSource _searchCts;

    private double? _userLatitude;
    private double? _userLongitude;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private ObservableRangeCollection<PlantPinDataModel> posts = new();

    [ObservableProperty]
    private ObservableCollection<RadiusOptionModel> radiusOptions = new();

    [ObservableProperty]
    private RadiusOptionModel selectedRadius;

    public CommunityPostsViewModel(DataService dataService)
    {
        _dataService = dataService;

        RadiusOptions = new ObservableCollection<RadiusOptionModel>
        {
            new() { Title = "Усі", Km = null, IsSelected = true },
            new() { Title = "1 км", Km = 1 },
            new() { Title = "5 км", Km = 5 },
            new() { Title = "10 км", Km = 10 },
            new() { Title = "25 км", Km = 25 },
            new() { Title = "50 км", Km = 50 }
        };

        SelectedRadius = RadiusOptions.FirstOrDefault(x => x.IsSelected);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;

        await TryLoadUserLocationAsync();
        await RefreshAsync();
    }

    private async Task TryLoadUserLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                return;

            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Medium));

            if (location != null)
            {
                _userLatitude = location.Latitude;
                _userLongitude = location.Longitude;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CommunityPostsViewModel.TryLoadUserLocationAsync] {ex.Message}");
        }
    }

    private void ResetAndClear()
    {
        _currentPage = 1;
        _hasMoreItems = true;
        Posts.Clear();
        IsEmpty = false;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsLoading)
            return;

        ResetAndClear();
        await LoadNextPageAsync();
    }

    [RelayCommand]
    public async Task LoadMoreAsync()
    {
        if (IsLoading || _isLoadingMore || !_hasMoreItems)
            return;

        await LoadNextPageAsync();
    }

    private async Task LoadNextPageAsync()
    {
        if (IsLoading || _isLoadingMore)
            return;

        try
        {
            _isLoadingMore = true;
            IsLoading = true;

            var result = await _dataService.GetCommunityPlantPinsAsync(
                ElementsOnPage,
                _currentPage,
                SearchText,
                SelectedRadius?.Km,
                _userLatitude,
                _userLongitude);

            if (result == null || result.Count == 0)
            {
                _hasMoreItems = false;
                IsEmpty = Posts.Count == 0;
                return;
            }

            Posts.AddRange(result);

            if (result.Count < ElementsOnPage)
                _hasMoreItems = false;

            _currentPage++;
            IsEmpty = Posts.Count == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CommunityPostsViewModel.LoadNextPageAsync] {ex.Message}");
        }
        finally
        {
            _isLoadingMore = false;
            IsLoading = false;
        }
    }

    public async Task SetSearchAsync(string text)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(400, _searchCts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        SearchText = text?.Trim();
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task SelectRadiusAsync(RadiusOptionModel option)
    {
        if (option == null)
            return;

        foreach (var item in RadiusOptions)
            item.IsSelected = false;

        option.IsSelected = true;
        SelectedRadius = option;

        await RefreshAsync();
    }

    [RelayCommand]
    private void ToggleExpand(PlantPinDataModel pin)
    {
        if (pin == null)
            return;

        pin.IsExpanded = !pin.IsExpanded;
    }

    [RelayCommand]
    private async Task OpenCommentsAsync(PlantPinDataModel post)
    {
        try
        {
            if (post == null)
                return;

            CommentsView commentsView = new CommentsView(_dataService);
            await commentsView.InitializeAsync(post);
            await Shell.Current.Navigation.PushModalAsync(commentsView);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CommunityPostsViewModel.OpenCommentsAsync] {ex.Message}");
        }
    }
}