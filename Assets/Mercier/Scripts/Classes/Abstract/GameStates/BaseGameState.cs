using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes.Abstract.GameStates
{
    public abstract class BaseGameState
    {
        public abstract void EnterState(GameManager gameManager);

        public abstract void Update(GameManager gameManager);

        public abstract void LateUpdate(GameManager gameManager);
    }
}

