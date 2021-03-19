using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class LimitedTurretRotation : BaseTurret
    {
        public override void TurretAttack()
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _turretStats.fireRate;

                OnTurretAttack();
            }
        }
    }
}

