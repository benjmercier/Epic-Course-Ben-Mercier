using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class LimitedTurretRotation : BaseTurret
    {
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;

        public override void TurretAttack()
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _fireRate;

                OnTurretAttack();
            }
        }
    }
}

