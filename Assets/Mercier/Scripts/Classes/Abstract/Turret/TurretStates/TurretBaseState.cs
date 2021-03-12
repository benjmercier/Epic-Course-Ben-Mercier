using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret.TurretStates
{
    public abstract class TurretBaseState
    {
        public abstract void EnterState(Turret turret);

        public abstract void Update(Turret turret);

        public abstract void LateUpdate(Turret turret);
    }
}

