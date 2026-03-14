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
            // Анімуємо IdleContainer (плавне гойдання)
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
            ShowDamageNumber(damage, color) // Передаємо колір сюди
        );
    }
    async Task EnemyShake()
    {
        // Анімуємо внутрішній EnemyContainer, щоб не перебивати Idle
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
        // Отримуємо центр контейнера
        double centerX = ParticleContainer.Width / 2;
        double centerY = ParticleContainer.Height / 2;

        // Якщо Width ще не провантажився (на самому початку), використовуємо приблизне значення
        if (centerX <= 0) centerX = 150;
        if (centerY <= 0) centerY = 100;

        var tasks = new List<Task>();

        for (int i = 0; i < 15; i++)
        {
            var p = new BoxView
            {
                Color = color,
                WidthRequest = 6,
                HeightRequest = 6,
                CornerRadius = 3,
                Opacity = 1
            };

            // Встановлюємо початкову позицію в центр
            AbsoluteLayout.SetLayoutBounds(p, new Rect(centerX, centerY, 6, 6));
            AbsoluteLayout.SetLayoutFlags(p, AbsoluteLayoutFlags.None);

            ParticleContainer.Children.Add(p);

            double angle = rng.NextDouble() * 2 * Math.PI;
            double dist = rng.Next(40, 130); // Радіус розльоту

            double targetX = centerX + Math.Cos(angle) * dist;
            double targetY = centerY + Math.Sin(angle) * dist;

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

        // 1. Початок атаки
        await ShowAttack("Strong Attack!");

        // 2. Розрахунок шансу влучання
        if (rng.NextDouble() > 0.4)
        {
            // Влучили! Колір можна не передавати (буде жовтий) або поставити DarkRed
            await EnemyDamageSequence(25, Colors.OrangeRed);
        }
        else
        {
            // Промах
            await ShowAttack("Miss!");
        }

        // 3. КРИТИЧНА ПЕРЕВІРКА: чи живий ще ворог?
        if (enemyHp <= 0)
        {
            await ShowWinScreen();
            return; // Виходимо з методу, хід ворога не починається
        }

        // 4. Якщо ворог вижив — передаємо йому хід
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

        // Ефекти на початку ходу
        foreach (var effect in enemyEffects.ToList())
        {
            int dmg = 0;
            Color effectColor = Colors.White;
            string effectName = "";

            if (effect.Type == EffectType.Bleed)
            {
                dmg = 7; // Урон від кровотечі
                effectColor = Colors.Crimson;
                effectName = "Bleed";
            }
            else if (effect.Type == EffectType.Poison)
            {
                dmg = 4; // Урон від отрути
                effectColor = Colors.LimeGreen;
                effectName = "Poison";
            }

            enemyHp = Math.Max(enemyHp - dmg, 0);
            EnemyHpBar.Progress = (double)enemyHp / enemyMaxHp;

            // Запускаємо візуальні ефекти тіка паралельно
            await Task.WhenAll(
                PlayParticleEffect(effectColor),
                ShowDamageNumber(dmg, effectColor),
                EnemyShake() // Додамо легке трясіння при тіку
            );

            effect.Duration--;
            if (effect.Duration <= 0) enemyEffects.Remove(effect);

            UpdateIcons();
            await Task.Delay(400); // Коротка пауза між різними ефектами
        }
        if (enemyHp <= 0)
        {
            await ShowAttack("Enemy Defeated!");
            await Task.Delay(500); // Коротка пауза для драматизму
            await ShowWinScreen(); // Показуємо вікно перемоги
            return;
        }

        await Task.Delay(600);

        // Хід ворога (атака гравця)
        await ShowAttack("Enemy Counter!");
        playerHp = Math.Max(playerHp - 10, 0);
        PlayerHpBar.Progress = (double)playerHp / playerMaxHp;

        playerTurn = true;
    }
    void UpdateIcons()
    {
        BleedIcon.IsVisible = enemyEffects.Any(x => x.Type == EffectType.Bleed);
        PoisonIcon.IsVisible = enemyEffects.Any(x => x.Type == EffectType.Poison);
    }
    async Task ShowDamageNumber(int damage, Color color = null)
    {
        // Якщо колір не передано, використовуємо стандартний жовтий
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
    async Task ShowAttack(string text)
    {
        AttackText.Text = text;
        AttackText.IsVisible = true;
        await Task.Delay(700);
        AttackText.IsVisible = false;
    }
    async Task ShowWinScreen()
    {
        playerTurn = false; // Блокуємо будь-які натискання

        WinPanel.IsVisible = true;
        await WinPanel.FadeTo(1, 500); // Плавна поява за пів секунди

        // Можна додати невеличку анімацію збільшення
        await WinPanel.ScaleTo(1.1, 200);
        await WinPanel.ScaleTo(1.0, 100);
    }

    // Обробник натискання кнопки на панелі перемоги
    async void OnReturnClicked(object sender, EventArgs e)
    {
        // Повертаємося на попередню сторінку (до персонажа)
        await Navigation.PopAsync();
    }
    #endregion
}

public class StatusEffect { public EffectType Type { get; set; } public int Duration { get; set; } }
public enum EffectType { Bleed, Poison }