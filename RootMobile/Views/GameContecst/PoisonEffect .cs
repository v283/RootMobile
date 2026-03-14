using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.GameContecst
{
    public class PoisonEffect : IStatusEffect
    {
        public string Name => "Poison";

        public int Duration { get; set; }

        private int damage;

        public PoisonEffect(int duration, int damagePerTick)
        {
            Duration = duration;
            damage = damagePerTick;
        }

        public void Apply(Character target)
        {
            Console.WriteLine($"{target.Name} отруєний!");
        }

        public void Tick(Character target)
        {
            target.CurrentHp -= damage;
            Console.WriteLine($"{target.Name} отримує {damage} шкоди від отрути.");
        }
    }
}
