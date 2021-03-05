using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class ActiveAction <T>
    {
        private T _newAction;
        private T _setAction;

        public ActiveAction(T newAction, T setAction)
        {
            this._newAction = newAction;
            this._setAction = setAction;

            _newAction = _setAction;
        }

        public T ReturnActiveAction()
        {
            return _newAction;
        }
    }
}

