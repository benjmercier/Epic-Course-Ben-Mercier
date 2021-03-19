using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class FullTurretRotation : BaseTurret
    {
        protected override bool ReturnTargetInLineOfSight(Vector3 targetPos)
        {
            return true;
        }
    }
}

