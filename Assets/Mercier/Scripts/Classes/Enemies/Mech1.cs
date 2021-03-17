using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Mercier.Scripts.Classes
{
    public class Mech1 : Enemy
    {
        [Space, SerializeField]
        private ParentConstraint _parentConstraint;
    }
}


