using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.GameContecst.Attacks
{
    public class BleedAttack : IAttack
    {
        public void Execute(Character attacker, Character target)
        {
            target.TakeDamage(attacker.Attack);

            var bleed = new BleedEffect(3, attacker.Attack / 2);
            target.AddEffect(bleed);
        }
    }
}
