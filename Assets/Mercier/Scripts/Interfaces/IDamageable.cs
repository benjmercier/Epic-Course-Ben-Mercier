using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Interfaces
{
    public interface IDamageable<T>
    {
        T Health { get; set; }
        T Armor { get; set; }

        void OnDamage(T health, T armor, T damageAmount, out T curHealth, out T curArmor);
    }
}


