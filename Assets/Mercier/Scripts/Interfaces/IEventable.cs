using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Interfaces
{
    public interface IEventable
    {
        void OnEnable();
        void OnDisable();
    }
}

