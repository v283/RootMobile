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
            // јн≥муЇмо IdleContainer (плавне гойданн€)
            await IdleContainer.TranslateTo(0, -8, 1200, Easing.SinInOut);
            await IdleContainer.TranslateTo(0, 8, 1200, Easing.SinInOut);
        }
    }

    async Task EnemyDamageSequence(int damage)
    {
        enemyHp = Math.Max(enemyHp - damage, 0);
        EnemyHpBar.Progress = (double)enemyHp / enemyMaxHp;

        await Task.WhenAll(
            EnemyDamageSprite.FadeTo(1, 100).ContinueWith(t => EnemyDamageSprite.FadeTo(0, 200)),
            EnemyShake(),
            ShowDamageNumber(damage)
        );
    }

    async Task EnemyShake()
    {
        // јн≥муЇмо внутр≥шн≥й EnemyContainer, щоб не перебивати Idle
        for (int i = 0; i < 2; i++)
        {
            await EnemyContainer.TranslateTo(-12, 0, 50);
            await EnemyContainer.TranslateTo(12, 0, 50);
        }
        await EnemyContainer.TranslateTo(0, 0, 50);
    }

    async Task ShowDamageNumber(int damage)
    {
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
        var tasks = new List<Task>();
        for (int i = 0; i < 15; i++)
        {
            var p = new BoxView { Color = color, WidthRequest = 6, HeightRequest = 6, CornerRadius = 3 };
            ParticleContainer.Children.Add(p);

            double angle = rng.NextDouble() * 2 * Math.PI;
            double dist = rng.Next(30, 120);

            tasks.Add(Task.Run(async () => {
                await Task.WhenAll(
                    p.TranslateTo(Math.Cos(angle) * dist, Math.Sin(angle) * dist, 600, Easing.CubicOut),
                    p.FadeTo(0, 600)
                );
                MainThread.BeginInvokeOnMainThread(() => ParticleContainer.Children.Remove(p));
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
        if (rng.NextDouble() > 0.4) await EnemyDamageSequence(25);
        else await ShowAttack("Miss!");
        await EnemyTurn();
    }

    async void QuickAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Quick Attack!");
        await EnemyDamageSequence(10);
        await EnemyTurn();
    }

    async void BleedAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Bleeding strike!");
        await Task.WhenAll(EnemyDamageSequence(5), PlayParticleEffect(Colors.Red));
        AddEffect(EffectType.Bleed, 3);
        await EnemyTurn();
    }

    async void PoisonAttack(object sender, EventArgs e)
    {
        if (!playerTurn) return;
        await ShowAttack("Poison gas!");
        await Task.WhenAll(EnemyDamageSequence(5), PlayParticleEffect(Colors.GreenYellow));
        AddEffect(EffectType.Poison, 4);
        await EnemyTurn();
    }

    void AddEffect(EffectType type, int duration)
    {
        enemyEffects.Add(new StatusEffect { Type = type, Duration = duration });
        UpdateIcons();
    }

    async Task EnemyTurn()
    {
        playerTurn = false;

        // ≈фекти на початку ходу
        foreach (var effect in enemyEffects.ToList())
        {
            int dmg = effect.Type == EffectType.Bleed ? 5 : 3;
            enemyHp = Math.Max(enemyHp - dmg, 0);
            await PlayParticleEffect(effect.Type == EffectType.Bleed ? Colors.Red : Colors.Green);
            effect.Duration--;
            if (effect.Duration <= 0) enemyEffects.Remove(effect);
        }

        EnemyHpBar.Progress = (double)enemyHp / enemyMaxHp;
        UpdateIcons();
        await Task.Delay(600);

        if (enemyHp > 0)
        {
            await ShowAttack("Enemy Counter!");
            playerHp = Math.Max(playerHp - 10, 0);
            PlayerHpBar.Progress = (double)playerHp / playerMaxHp;
        }

        playerTurn = true;
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