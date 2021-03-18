using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Enemy.EnemyStates
{
    public abstract class BaseEnemyState
    {
        public abstract void EnterState(BaseEnemy enemy);

        public abstract void Update(BaseEnemy enemy);

        public abstract void LateUpdate(BaseEnemy enemy);
    }
}

