using CommunityToolkit.Maui.Views;
using System.Text;

namespace RootMobile.Views.TreeGameContext;

public partial class DialogPopUp : Popup
{
    private string fullText;
    private bool isTyping = true;
    private bool textFinished = false;

    public DialogPopUp(string text)
    {
        InitializeComponent();

        fullText = text;

        Opened += OnPopupOpened; // ← ось правильно

        StartTyping();
    }

    // ефект друку тексту
    private async void StartTyping()
    {
        StringBuilder builder = new();

        foreach (char c in fullText)
        {
            if (!isTyping)
                break;

            builder.Append(c);
            DialogText.Text = builder.ToString();

            await Task.Delay(25); // швидкість друку
        }

        isTyping = false;
        textFinished = true;
    }
    private async void OnPopupOpened(object? sender, EventArgs e)
    {
        PopupFrame.Scale = 0.8;
        PopupFrame.Opacity = 0;

        await Task.WhenAll(
            PopupFrame.ScaleTo(1, 200, Easing.CubicOut),
            PopupFrame.FadeTo(1, 200)
        );
    }
    // обробка тапів
    private void OnTapped(object sender, EventArgs e)
    {
        // 1-й тап — показати текст одразу
        if (!textFinished)
        {
            isTyping = false;
            DialogText.Text = fullText;
            textFinished = true;
            return;
        }

        // 2-й тап — закрити popup
        Close();
    }
}