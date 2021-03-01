using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.ScriptableObjects;

namespace Mercier.Scripts.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private static int warFunds;
        public int _currentWarFunds;

        public void Purchased(int cost)
        {
            _currentWarFunds -= cost;
        }
    }
}
