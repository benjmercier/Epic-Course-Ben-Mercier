using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Managers
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        public List<string> layers = new List<string>();

        public List<string> parameters = new List<string>();

        [Header("Animation Layers")]
        [SerializeField]
        private string _baseLayer = "Base Layer";
        public string BaseLayer { get { return _baseLayer; } }

        [SerializeField]
        private string _firingUpperLayer = "Firing Upper";
        public string FiringUpperLayer { get { return _firingUpperLayer; } }

        [Header("Animation Parameters")]
        [SerializeField]
        private string _isIdleParam = "isIdle";
        public string IsIdleParam { get { return _isIdleParam; } }

        [SerializeField]
        private string _isFiringParam = "isFiring";
        public string IsFiringParam { get { return _isFiringParam; } }

        [SerializeField]
        private string _isDestroyedParam = "isDestroyed";
        public string IsDestroyedParam { get { return _isDestroyedParam; } }
    }
}

