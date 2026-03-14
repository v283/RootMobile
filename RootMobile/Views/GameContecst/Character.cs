using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.GameContecst
{
    public class Character
    {
        public string Name { get; set; }

        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }

        public int Attack { get; set; }
        public int Defense { get; set; }

        public List<IStatusEffect> ActiveEffects { get; private set; } = new();

        public void TakeDamage(int damage)
        {
            int finalDamage = Math.Max(damage - Defense, 0);
            CurrentHp -= finalDamage;

            Console.WriteLine($"{Name} отримав {finalDamage} шкоди. HP: {CurrentHp}");
        }

        public void AddEffect(IStatusEffect effect)
        {
            ActiveEffects.Add(effect);
            effect.Apply(this);
        }

        public void UpdateEffects()
        {
            foreach (var effect in ActiveEffects.ToList())
            {
                effect.Tick(this);
                effect.Duration--;

                if (effect.Duration <= 0)
                {
                    ActiveEffects.Remove(effect);
                    Console.WriteLine($"{effect.Name} закінчився.");
                }
            }
        }
    }
}
