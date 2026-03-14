using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.GameContecst
{
    public class BleedEffect : IStatusEffect
    {
        public string Name => "Bleeding";

        public int Duration { get; set; }

        private int damagePerTick;

        public BleedEffect(int duration, int damage)
        {
            Duration = duration;
            damagePerTick = damage;
        }

        public void Apply(Character target)
        {
            Console.WriteLine($"{target.Name} почав кровоточити!");
        }

        public void Tick(Character target)
        {
            target.CurrentHp -= damagePerTick;

            Console.WriteLine($"{target.Name} отримує {damagePerTick} шкоди від кровотечі.");
        }
    }
}
