using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Interfaces
{
    public interface IDamageable<T>
    {
        void OnDamageReceived(T health, T armor, T damageAmount, out T curHealth, out T curArmor);
    }
}


