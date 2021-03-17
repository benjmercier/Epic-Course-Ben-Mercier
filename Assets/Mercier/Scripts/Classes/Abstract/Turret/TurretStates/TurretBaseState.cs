using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public abstract class TurretBaseState
    {
        public abstract void EnterState(BaseTurret turret);

        public abstract void Update(BaseTurret turret);

        public abstract void LateUpdate(BaseTurret turret);
    }
}

