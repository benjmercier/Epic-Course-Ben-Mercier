using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mercier.Scripts.ScriptableObjects;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Managers
{
    public class DatabaseManager : MonoSingleton<DatabaseManager>
    {
        public List<Database> databases = new List<Database>();

        private void OnEnable()
        {
            UIManager.onRequestSprietFromDatabase += ReturnTurretSprite;
            UIManager.onRequestCostFromDatabase += ReturnTurretCost;
        }

        private void OnDisable()
        {
            UIManager.onRequestSprietFromDatabase -= ReturnTurretSprite;
            UIManager.onRequestCostFromDatabase -= ReturnTurretCost;
        }

        private Sprite ReturnTurretSprite(int index)
        {
            if (databases[1].databaseList[index].prefab.TryGetComponent(out BaseTurret baseTurret))
            {
                return baseTurret.TurretStats.currentSprite;
            }

            return null;
        }

        private int ReturnTurretCost(int index)
        {
            if (databases[1].databaseList[index].prefab.TryGetComponent(out BaseTurret baseTurret))
            {
                return baseTurret.TurretStats.cost;
            }

            return 0;
        }
    }
}

