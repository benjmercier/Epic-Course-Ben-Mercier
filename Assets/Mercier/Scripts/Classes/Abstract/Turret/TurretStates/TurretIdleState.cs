using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretIdleState : BaseTurretState
    {
        public override void EnterState(BaseTurret turret)
        {
            
        }

        public override void Update(BaseTurret turret)
        {
            if (turret.ActiveTarget != null)
            {
                turret.TransitionToState(turret.turretAttackingState);
            }
        }

        public override void LateUpdate(BaseTurret turret)
        {
            
        }
    }
}

