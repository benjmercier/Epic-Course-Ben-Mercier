using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes.Abstract.Enemy.EnemyStates
{
    public class EnemyAttackingState : BaseEnemyState
    {
        public override void EnterState(BaseEnemy enemy)
        {
            enemy.EnemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, true);
        }

        public override void Update(BaseEnemy enemy)
        {
            if (enemy.ActiveTarget == null)
            {
                enemy.TransitionToState(enemy.enemyCoolDownState);
            }
        }

        public override void LateUpdate(BaseEnemy enemy)
        {
            if (enemy.ActiveTarget != null)
            {
                // turret uses RotationTarget bc child RotationTarget
                enemy.RotateToTarget(enemy.ActiveTarget.transform.position);
            }
        }
    }
}

