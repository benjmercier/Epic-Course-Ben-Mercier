using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Mercier.Scripts.Classes.Custom.UI
{   [System.Serializable]
    public class LevelStatusUI
    {
        public GameObject baseMenu;

        public TextMeshProUGUI inputToStart;
        public TextMeshProUGUI startTMP;
        public TextMeshProUGUI startTimerTMP;
        public TextMeshProUGUI completeTMP;

        public void EnableStartTimer(bool enable)
        {
            ToggleInputToStart(!enable);

            ToggleStartTMP(enable);
        }

        public void ToggleInputToStart(bool enable)
        {
            inputToStart.gameObject.SetActive(enable);
        }

        public void ToggleStartTMP(bool enable)
        {
            startTMP.gameObject.SetActive(enable);
        }
    }
}

