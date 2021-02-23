using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public static class UtilityHelper
    {
        public static WaitForSeconds AssignWaitForSeconds(float wait)
        {
            return new WaitForSeconds(wait);
        }
    }
}

