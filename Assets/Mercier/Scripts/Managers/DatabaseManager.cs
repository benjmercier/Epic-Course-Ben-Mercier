using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.ScriptableObjects;

namespace Mercier.Scripts.Managers
{
    public class DatabaseManager : MonoSingleton<DatabaseManager>
    {
        public List<Database> databases = new List<Database>();
    }
}

