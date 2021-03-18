using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret.TurretStates;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes.Custom;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class BaseTurret : BaseBehavior<BaseTurretState>
    { 
        private BaseTurretState _currentTurretState;
        public BaseTurretState CurrentTurretState { get { return _currentTurretState; } }
        public readonly TurretIdleState turretIdleState = new TurretIdleState();
        public readonly TurretCoolDownState turretCoolDownState = new TurretCoolDownState();
        public readonly TurretAttackingState turretAttackingState = new TurretAttackingState();
        public readonly TurretDestroyedState turretDestroyedState = new TurretDestroyedState();

        [Space, SerializeField]
        protected TurretStats _turretStats;

        protected bool _canFire;
        protected bool _hasFired;

        public static event Action<GameObject, float> onTurretAttack;

        public override void OnEnable()
        {
            base.OnEnable();

            _turretStats.currentHealth = _turretStats.maxHealth;
            _turretStats.currentArmor = _turretStats.maxArmor;

            TransitionToState(turretIdleState);

            //Enemy.onEnemyDeath += AssignNewTarget;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            //Enemy.onEnemyDeath -= AssignNewTarget;
        }

        protected override void Update()
        {
            _currentTurretState.Update(this);
        }

        protected override void LateUpdate()
        {
            _currentTurretState.LateUpdate(this);
        }

        public override void TransitionToState(BaseTurretState state)
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
            onTurretAttack?.Invoke(_targeting.activeTarget,  _turretStats.attackStrength);
        }

        protected override void ReceiveDamage(GameObject damagedObj, float damageAmount)
        {
            if (this.gameObject == damagedObj)
            {
                OnDamageReceived(_turretStats.currentHealth, _turretStats.currentArmor, damageAmount, out _turretStats.currentHealth, out _turretStats.currentArmor);
            }
        }

        public override void OnDamageReceived(float health, float armor, float damageAmount, out float curHealth, out float curArmor)
        {
            if (armor > _zero)
            {
                armor -= damageAmount;

                _delta = health - armor;

                if (armor < _zero)
                {
                    armor = _zero;
                }

                curArmor = armor;
            }
            else
            {
                if (armor != _zero)
                {
                    armor = _zero;
                }

                curArmor = armor;

                _delta = _turretStats.maxHealth;
            }

            health -= (_delta / _turretStats.maxHealth) * damageAmount;

            curHealth = health;

            if (curHealth <= 0)
            {
                curHealth = 0;

                OnObjDestroyed(this.gameObject, _turretStats.destroyedPenalty);
            }
        }
    }
}

