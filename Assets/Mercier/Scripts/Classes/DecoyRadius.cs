using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes
{
    public class DecoyRadius : MonoBehaviour, IEventable
    {
        private Material _material;

        private void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void OnEnable()
        {            
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

