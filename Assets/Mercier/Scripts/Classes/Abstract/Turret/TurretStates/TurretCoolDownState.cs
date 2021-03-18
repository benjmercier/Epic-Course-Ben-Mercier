using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretCoolDownState : BaseTurretState
    {
        public override void EnterState(BaseTurret turret)
        {
            
        }

        public override void Update(BaseTurret turret)
        {
            if (turret.IsAtStart())
            {
                turret.TransitionToState(turret.turretIdleState);
            }
            else if (turret.ActiveTarget != null)
            {
                turret.TransitionToState(turret.turretAttackingState);
            }
        }

        public override void LateUpdate(BaseTurret turret)
        {
            if (!turret.IsAtStart())
            {
                turret.RotateToStart();
            } 
        }
    }
}


