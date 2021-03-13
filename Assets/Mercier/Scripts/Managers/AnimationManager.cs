using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Managers
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        public List<string> layers = new List<string>();

        public List<string> parameters = new List<string>();
    }
}

