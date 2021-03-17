using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret.TurretStates;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class BaseTurret : SharedBehaviors
    { 
        private TurretBaseState _currentTurretState;
        public TurretBaseState CurrentTurretStat { get { return _currentTurretState; } }
        public readonly TurretIdleState turretIdleState = new TurretIdleState();
        public readonly TurretCoolDownState turretCoolDownState = new TurretCoolDownState();
        public readonly TurretAttackingState turretAttackingState = new TurretAttackingState();
        public readonly TurretDestroyedState turretDestroyedState = new TurretDestroyedState();

        [SerializeField]
        protected readonly int _turretID;
        [SerializeField]
        protected readonly int _turretCost;

        protected bool _canFire;
        protected bool _hasFired;

        public static event Action<GameObject, float> onTurretAttack;

        public override void OnEnable()
        {
            base.OnEnable();

            TransitionToState(turretIdleState);

            Enemy.onEnemyDeath += AssignNewTarget;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            Enemy.onEnemyDeath -= AssignNewTarget;
        }

        protected override void Update()
        {
            _currentTurretState.Update(this);
        }

        protected virtual void LateUpdate()
        {
            _currentTurretState.LateUpdate(this);
        }

        public void TransitionToState(TurretBaseState state)
        {
            _currentTurretState = state;
            _currentTurretState.EnterState(this);
        }

        protected virtual void ActivateTurret(bool activate)
        {
            _canFire = activate;

            if (!activate)
            {
                DisengageTarget();
            }
        }

        protected abstract void EngageTarget();

        protected abstract void DisengageTarget();

        public abstract void TurretAttack();

        protected virtual void OnTurretAttack()
        {
            onTurretAttack?.Invoke(_targeting.activeTarget,  _attackStrength);
        }
    }
}

