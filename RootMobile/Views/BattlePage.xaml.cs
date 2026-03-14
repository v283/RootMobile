using Microsoft.Maui.Layouts;

namespace RootMobile.Views;

public partial class NewPage1 : ContentPage
{
    int enemyHp = 100, enemyMaxHp = 100;
    int playerHp = 100, playerMaxHp = 100;
    bool playerTurn = true;
    Random rng = new();
    List<StatusEffect> enemyEffects = new();

    public NewPage1()
    {
        InitializeComponent();
        _ = StartIdleAnimation();
    }

    #region Animation Core
    async Task StartIdleAnimation()
    {
        while (true)
        {
            await IdleContainer.TranslateTo(0, -8, 1200, Easing.SinInOut);
            await IdleContainer.TranslateTo(0, 8, 1200, Easing.SinInOut);
        }
    }

    async Task EnemyDamageSequence(int damage, Color color = null)
    {
        enemyHp = Math.Max(enemyHp - damage, 0);
        EnemyHpBar.Progress = (double)enemyHp / enemyMaxHp;

        await Task.WhenAll(
            EnemyDamageSprite.FadeTo(1, 100).ContinueWith(t => MainThread.BeginInvokeOnMainThread(() => EnemyDamageSprite.FadeTo(0, 200))),
            EnemyShake(),
            ShowDamageNumber(damage, color)
        );
    }

    async Task EnemyShake()
    {
        for (int i = 0; i < 2; i++)
        {
            await EnemyContainer.TranslateTo(-12, 0, 50);
            await EnemyContainer.TranslateTo(12, 0, 50);
        }
        await EnemyContainer.TranslateTo(0, 0, 50);
    }

    async Task ShowDamageNumber(int damage, Color color = null)
    {
        DamageText.TextColor = color ?? Colors.Yellow;
        DamageText.Text = $"-{damage}";
        DamageText.IsVisible = true;
        DamageText.Opacity = 1;
        DamageText.TranslationY = 0;

        await Task.WhenAll(
            DamageText.TranslateTo(0, -80, 700, Easing.CubicOut),
            DamageText.FadeTo(0, 700)
        );
        DamageText.IsVisible = false;
    }

    async Task PlayParticleEffect(Color color)
    {
        double centerX = ParticleContainer.Width / 2;
        double centerY = ParticleContainer.Height / 2;
        if (centerX <= 0) centerX = 150;
        if (centerY <= 0) centerY = 100;

        var tasks = new List<Task>();
        for (int i = 0; i < 15; i++)
        {
            var p = new BoxView { Color = color, WidthRequest = 6, HeightRequest = 6, CornerRadius = 3 };
            AbsoluteLayout.SetLayoutBounds(p, new Rect(centerX, centerY, 6, 6));
            ParticleContainer.Children.Add(p);

            double angle = rng.NextDouble() * 2 * Math.PI;
            double dist = rng.Next(40, 130);

            tasks.Add(MainThread.InvokeOnMainThreadAsync(async () => {
                await Task.WhenAll(
                    p.TranslateTo(Math.Cos(angle) * dist, Math.Sin(angle) * dist, 600, Easing.CubicOut),
                    p.FadeTo(0, 600)
                );
                ParticleContainer.Children.Remove(p);
            }));
        }
        await Task.WhenAll(tasks);
    }
    #endregion

    #region Battle Logic
    async void StrongAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Strong Attack!");
        if (rng.NextDouble() > 0.4) await EnemyDamageSequence(25, Colors.OrangeRed);
        else await ShowAttack("Miss!");

        if (enemyHp <= 0) { await HandleVictory(); return; }
        await EnemyTurn();
    }

    async void QuickAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Quick Attack!");
        await EnemyDamageSequence(10, Colors.White);

        if (enemyHp <= 0) { await HandleVictory(); return; }
        await EnemyTurn();
    }

    async void BleedAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Bleeding strike!");
        await EnemyDamageSequence(5, Colors.Crimson);
        AddEffect(EffectType.Bleed, 3);

        if (enemyHp <= 0) { await HandleVictory(); return; }
        await EnemyTurn();
    }

    async void PoisonAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Poison gas!");
        await EnemyDamageSequence(5, Colors.LimeGreen);
        AddEffect(EffectType.Poison, 4);

        if (enemyHp <= 0) { await HandleVictory(); return; }
        await EnemyTurn();
    }

    async Task EnemyTurn()
    {
        playerTurn = false;

        foreach (var effect in enemyEffects.ToList())
        {
            int dmg = effect.Type == EffectType.Bleed ? 7 : 4;
            Color color = effect.Type == EffectType.Bleed ? Colors.Crimson : Colors.LimeGreen;

            await Task.WhenAll(
                PlayParticleEffect(color),
                EnemyDamageSequence(dmg, color)
            );

            effect.Duration--;
            if (effect.Duration <= 0) enemyEffects.Remove(effect);
            UpdateIcons();
            await Task.Delay(400);

            if (enemyHp <= 0) { await HandleVictory(); return; }
        }

        await Task.Delay(600);
        await ShowAttack("Enemy Counter!");
        playerHp = Math.Max(playerHp - 10, 0);
        PlayerHpBar.Progress = (double)playerHp / playerMaxHp;
        playerTurn = true;
    }

    async Task HandleVictory()
    {
        await ShowAttack("Enemy Defeated!");
        await Task.Delay(500);
        await ShowWinScreen();
    }

    async Task ShowWinScreen()
    {
        playerTurn = false;
        WinPanel.IsVisible = true;
        await WinPanel.FadeTo(1, 500);
        await WinPanel.ScaleTo(1.05, 200);
        await WinPanel.ScaleTo(1.0, 100);
    }

    async void OnReturnClicked(object sender, EventArgs e) => await Navigation.PopAsync();

    void AddEffect(EffectType type, int duration)
    {
        enemyEffects.Add(new StatusEffect { Type = type, Duration = duration });
        UpdateIcons();
    }

    void UpdateIcons()
    {
        BleedIcon.IsVisible = enemyEffects.Any(x => x.Type == EffectType.Bleed);
        PoisonIcon.IsVisible = enemyEffects.Any(x => x.Type == EffectType.Poison);
    }

    async Task ShowAttack(string text)
    {
        AttackText.Text = text;
        AttackText.IsVisible = true;
        await Task.Delay(700);
        AttackText.IsVisible = false;
    }
    #endregion
}

public class StatusEffect { public EffectType Type { get; set; } public int Duration { get; set; } }
public enum EffectType { Bleed, Poison }