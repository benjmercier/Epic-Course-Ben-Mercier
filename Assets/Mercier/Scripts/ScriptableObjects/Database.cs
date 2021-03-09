using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewDatabase.asset", menuName = "Scriptable Objects/Database")]
    public class Database : ScriptableObject
    {
        public List<DatabaseItem> databaseList;
    }
}

