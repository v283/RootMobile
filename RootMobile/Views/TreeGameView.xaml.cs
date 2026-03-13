using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
namespace RootMobile.Views;

using RootMobile.Views.TreeGameContext;

public partial class TreeGameView : ContentPage
{
    private GameData gameData;
    public TreeGameView()
    {
        gameData = new GameData();
        InitializeComponent();
        TreeSprite.Source = gameData.userTree.nameOfImage;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (gameData.gameState == GameState.Wellcome)
        {
            ShowDialog(0, true);
        }
    }
    private string fullText;
    private bool isTyping;
    private bool textFinished;
    private int currentIndexPhraze;

    private float positionX = 4;
    private float positionY = 10;
    private void OnDialogTapped(object sender, EventArgs e)
    {
        // 1 тап — показати весь текст
        if (!textFinished)
        {
            isTyping = false;
            DialogText.Text = fullText;
            textFinished = true;
            return;
        }

        // 2 тап — закрити
        DialogLayer.IsVisible = false;
        currentIndexPhraze++;
        if (currentIndexPhraze < gameData.wellcomeTextList.Count)
        {
            ShowDialog(currentIndexPhraze, false);
        }
    }
    public async void ShowDialog(int indexOfFraze, bool withoutFone, float positionX, int PositionY)
    {
        this.positionX = positionX;
        this.positionY = PositionY;
        ShowDialog(indexOfFraze, withoutFone);
    }
    public async void ShowDialog(int indexOfFraze, bool withoutFone)
    {
        DialogLayer.IsVisible = true;
        AbsoluteLayout.SetLayoutBounds(
             DialogBox,
            new Rect(positionX, positionY, 350, -1));

        if (withoutFone)
            DialogLayer.BackgroundColor = Colors.Transparent;
        else
            DialogLayer.BackgroundColor = Color.FromArgb("#80000000");
        currentIndexPhraze = indexOfFraze;
        fullText = gameData.wellcomeTextList[currentIndexPhraze];
        isTyping = true;
        textFinished = false;

        DialogText.Text = "";

        await TypeText();
    }
    private async Task TypeText()
    {
        StringBuilder builder = new();

        foreach (char c in fullText)
        {
            if (!isTyping)
                break;

            builder.Append(c);
            DialogText.Text = builder.ToString();

            await Task.Delay(25);
        }

        DialogText.Text = fullText;
        isTyping = false;
        textFinished = true;
    }
    private void OnWaterClicked(object sender, EventArgs e)
    {
        AddXP(10);
    }
    private void OnFertilizeClicked(object sender, EventArgs e)
    {
        AddXP(25);
    }
    private int currentXP = 40;
    private int maxXP = 100;
    private int level = 1;
    private void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            level++;

            ShowDialog(0, false); // level up dialog
        }

        UpdateUI();
    }
    private void UpdateUI()
    {
        TreeProgress.Progress = (double)currentXP / maxXP;
        ProgressText.Text = $"{currentXP} / {maxXP}";
        LevelText.Text = level.ToString();
    }
}