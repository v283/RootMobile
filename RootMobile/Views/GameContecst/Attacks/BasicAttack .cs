using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile.Views.GameContecst.Attacks
{
    public class BasicAttack : IAttack
    {
        public void Execute(Character attacker, Character target)
        {
            target.TakeDamage(attacker.Attack);
        }
    }
}
