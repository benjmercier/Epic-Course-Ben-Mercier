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

        private void List()
        {
            var cost = databases[1].databaseList[0].prefab.GetComponent<BaseTurret>().TurretStats.cost;
        }

        private Sprite ReturnTurretSprite(int index)
        {
            return databases[1].databaseList[index].prefab.GetComponent<BaseTurret>().TurretStats.currentSprite;
        }

        private int ReturnTurretCost(int index)
        {
            return databases[1].databaseList[index].prefab.GetComponent<BaseTurret>().TurretStats.cost;
        }
    }
}

