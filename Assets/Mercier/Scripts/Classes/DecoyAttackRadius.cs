using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes
{
    public class DecoyAttackRadius : MonoBehaviour, IEventable
    {
        private Material _material;

        public void OnEnable()
        {
            _material = GetComponent<Renderer>().material;
            TowerManager.onTurretPlacementColor += StatusColor;
        }

        public void OnDisable()
        {
            TowerManager.onTurretPlacementColor -= StatusColor;   
        }

        private void StatusColor(Color32 color)
        {
            _material.color = color;
        }
    }
}

