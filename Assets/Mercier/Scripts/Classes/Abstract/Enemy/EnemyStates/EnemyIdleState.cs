using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Enemy.EnemyStates
{
    public class EnemyIdleState : BaseEnemyState
    {
        public override void EnterState(BaseEnemy enemy)
        {
            
        }

        public override void Update(BaseEnemy enemy)
        {
            if (enemy.ActiveTarget != null)
            {
                enemy.TransitionToState(enemy.enemyAttackingState);
            }
        }

        public override void LateUpdate(BaseEnemy enemy)
        {
            
        }
    }
}

