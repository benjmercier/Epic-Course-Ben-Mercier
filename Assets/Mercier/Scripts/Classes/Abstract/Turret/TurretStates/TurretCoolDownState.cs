﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public class TurretCoolDownState : TurretBaseState
    {
        public override void EnterState(Turret turret)
        {
            
        }

        public override void Update(Turret turret)
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

        public override void LateUpdate(Turret turret)
        {
            if (!turret.IsAtStart())
            {
                turret.RotateToStart();
            } 
        }
    }
}


