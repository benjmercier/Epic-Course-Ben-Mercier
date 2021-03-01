using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewInventory.asset", menuName = "Scriptable Objects/New Inventory/Turret")]
    public class Inventory_Turret : ScriptableObject
    {
        public List<Turret> turretInventory = new List<Turret>();
    }
}

