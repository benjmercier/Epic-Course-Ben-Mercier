using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Enemy;

namespace Mercier.Scripts.Classes.Enemies
{
    public class Mech2_new : BaseEnemy
    {
        protected override void ReceiveDamage(GameObject damagedObj, float damageAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDamageReceived(float health, float armor, float damageAmount, out float curHealth, out float curArmor)
        {
            throw new System.NotImplementedException();
        }
    }
}


