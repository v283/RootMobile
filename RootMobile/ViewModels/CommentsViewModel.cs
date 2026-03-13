using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RootMobile.Models;
using RootMobile.Services;

namespace RootMobile.ViewModels
{
    public partial class CommentsViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private PlantPinDataModel post;

        [ObservableProperty]
        private ObservableCollection<PlantCommentModel> comments = new();

        [ObservableProperty]
        private string newCommentText;

        [ObservableProperty]
        private bool isLoading;

        public CommentsViewModel(IDataService dataService)
        {
            _dataService = (DataService)dataService;
        }

        public async Task InitializeAsync(PlantPinDataModel plantPost)
        {
            Post = plantPost;
            await LoadCommentsAsync();
        }

        [RelayCommand]
        private async Task LoadComments()
        {
            await LoadCommentsAsync();
        }

        private async Task LoadCommentsAsync()
        {
            try
            {
                if (Post == null)
                    return;

                IsLoading = true;
                Comments.Clear();

                var commentsList = await _dataService.GetPlantCommentsAsync(Post.Id);

                if (commentsList != null)
                {
                    foreach (var comment in commentsList)
                    {
                        Comments.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadCommentsAsync] Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddComment()
        {
            try
            {
                if (Post == null || string.IsNullOrWhiteSpace(NewCommentText))
                    return;

                IsLoading = true;

                var createdComment = await _dataService.AddPlantCommentAsync(Post.Id, NewCommentText.Trim());

                if (createdComment != null)
                {
                    Comments.Insert(0, createdComment);
                    NewCommentText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AddComment] Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}