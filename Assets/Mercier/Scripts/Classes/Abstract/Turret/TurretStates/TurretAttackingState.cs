﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretAttackingState : TurretBaseState
    {
        public override void EnterState(Turret turret)
        {
            
        }
        public override void Update(Turret turret)
        {
            if (turret.RotationTarget == null)
            {
                turret.TransitionToState(turret.turretCoolDownState);
            }
        }

        public override void LateUpdate(Turret turret)
        {
            if (turret.RotationTarget != null)
            {
                turret.RotateToTarget(turret.RotationTarget.transform.position);
            }
        }
    }
}
