﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretAttackingState : BaseTurretState
    {
        public override void EnterState(BaseTurret turret)
        {
            
        }
        public override void Update(BaseTurret turret)
        {
            if (turret.RotationTarget == null)
            {
                turret.ActivateTurret(false);
                turret.TransitionToState(turret.turretCoolDownState);

                return;
            }

            turret.ActivateTurret(true);
            turret.TurretAttack();
        }

        public override void LateUpdate(BaseTurret turret)
        {
            if (turret.RotationTarget != null)
            {
                turret.RotateToTarget(turret.RotationTarget.transform.position);
            }
        }
    }
}
