using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretIdleState : TurretBaseState
    {
        public override void EnterState(Turret turret)
        {
            
        }

        public override void Update(Turret turret)
        {
            if (turret.ActiveTarget != null)
            {
                turret.TransitionToState(turret.turretAttackingState);
            }
        }

        public override void LateUpdate(Turret turret)
        {
            
        }
    }
}

