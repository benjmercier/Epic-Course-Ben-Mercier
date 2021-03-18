using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes.Abstract.Enemy.EnemyStates
{
    public class EnemyCoolDownState : BaseEnemyState
    {
        public override void EnterState(BaseEnemy enemy)
        {
            enemy.EnemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, false);
        }

        public override void Update(BaseEnemy enemy)
        {
            if (enemy.IsAtStart())
            {
                enemy.TransitionToState(enemy.enemyIdleState);
            }
            else if (enemy.ActiveTarget != null)
            {
                enemy.TransitionToState(enemy.enemyAttackingState);
            }
        }

        public override void LateUpdate(BaseEnemy enemy)
        {
            enemy.RotateToStart();

            if (!enemy.IsAtStart())
            {
                
            }
        }
    }
}