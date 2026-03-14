using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views;

public partial class DoPlantView : ContentPage
{
    private double _lat;
    private double _lon;
    public DoPlantView(double lat, double lon)
    {
        InitializeComponent();
        _lat = lat;
        _lon = lon;
    }


    private async Task ChatBot(string prompt)
    {

        try {

            await Shell.Current.GoToAsync($"botpage?prompt={Uri.EscapeDataString(prompt)}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", "Не вдалося відкрити чат: " + ex.Message, "OK");
        }
    }

    private async void OnConsultAiClicked(object sender, EventArgs e)
    {
        string plantName = PlantEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(plantName))
        {
            await DisplayAlert("Увага", "Введіть назву рослини", "OK");
            return;
        }

        // 1. Геруємо детальний промпт
        string prompt = $"Привіт! Я хочу посадити рослину '{plantName}' за координатами: " +
                        $"широта {_lat}, довгота {_lon}. " +
                        $"Скажи, чи підходить ця локація для цієї рослини? " +
                        $"Які саме умови (ґрунт, сонце, вологість) мені потрібно створити, щоб вона прижилася?";

        // 2. Викликаємо метод переходу до чату, передаючи промпт
        await ChatBot(prompt);
    }
}