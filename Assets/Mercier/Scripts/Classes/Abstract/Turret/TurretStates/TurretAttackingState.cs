using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretAttackingState : TurretBaseState
    {
        public override void EnterState(BaseTurret turret)
        {
            
        }
        public override void Update(BaseTurret turret)
        {
            if (turret.RotationTarget == null)
            {
                turret.TransitionToState(turret.turretCoolDownState);
            }

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
