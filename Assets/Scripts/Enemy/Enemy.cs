using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable<float>
{
    [SerializeField]
    protected float _speed;
    [SerializeField]
    protected float _maxHealth;
    [SerializeField]
    protected float _attackStrength;
    [SerializeField]
    protected float _maxArmor;
    [SerializeField]
    protected int _currencyToReward;

    private float _delta;
    private float _zero = 0f;

    public float Health { get; set; }
    public float Armor { get; set; }

    protected void Awake()
    {
        Health = _maxHealth;
        Armor = _maxArmor;
    }

    protected virtual void Attack(float attackStrength)
    {

    }

    public virtual void OnDamage(float health, float armor, float damageAmount,  out float curHealth, out float curArmor)
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

            _delta = _maxHealth;
        }

        health -= (_delta / _maxHealth) * damageAmount;

        curHealth = health;
    }

    protected virtual void OnDeath(int reward)
    {
        // add reward to player currency

        Destroy(this.gameObject);
    }
}
