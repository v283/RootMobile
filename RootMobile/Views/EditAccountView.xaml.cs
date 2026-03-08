using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RootMobile.Models;
using RootMobile.Services;

namespace RootMobile.Views;

public partial class EditAccountView : ContentPage
{
    private readonly DataService _dataService;
    private string _selectedPhotoPath;
    private UserDataModel _userData;

    public EditAccountView(IDataService dataService, UserDataModel userData)
    {
        InitializeComponent();
        _dataService = (DataService)dataService;
        _userData = userData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_userData is not null)
        {
            NameEntry.Text = _userData.Name;
            PhoneEntry.Text = _userData.Phone;
            StatusEntry.Text = _userData.Status;

            if (!string.IsNullOrWhiteSpace(_userData.Image))
            {
                ProfileImage.Source = _userData.Image;
            }
        }
    }

    private async void BackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private async void ChangePhotoTapped(object sender, TappedEventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync();

            if (result == null)
                return;

            _selectedPhotoPath = result.FullPath;
            ProfileImage.Source = ImageSource.FromFile(result.FullPath);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void SaveTapped(object sender, EventArgs e)
    {
        try
        {
            if (_userData is null)
            {
                await DisplayAlert("Помилка", "User data not found", "OK");
                return;
            }

            string name = NameEntry.Text?.Trim();
            string phone = PhoneEntry.Text?.Trim();
            string status = StatusEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Validation", "Please enter your name", "OK");
                return;
            }

            _userData.Name = name;
            _userData.Phone = phone;
            _userData.Status = status;

            // Якщо користувач вибрав нове фото
            if (!string.IsNullOrWhiteSpace(_selectedPhotoPath))
            {
                // Вивантажуємо фото в Supabase Storage
                string imageUrl = await _dataService.UploadProfileImageAsync(_selectedPhotoPath);
            
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    _userData.Image = imageUrl; // Замінюємо локальний шлях на публічний URL
                }
                else
                {
                    await DisplayAlert("Помилка", "Failed to upload image to the server.", "OK");
                    return; // Зупиняємо збереження, якщо фото не вивантажилось
                }
            }

            // Зберігаємо оновлені дані в Supabase (PostgreSQL)
            await _dataService.UpdateUserProfileAsync(_userData);

            await DisplayAlert("Success", "Profile updated", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    
}