using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom.UI
{
    [System.Serializable]
    public class ArmoryUI
    {
        public GameObject baseMenu;
        public Image[] turretImages;
        public Button[] turretButtons;

        [ReadOnly]
        public Sprite currentSprite;

        [Header("Upgrade")]
        public Button upgradeButton;
        public Image upgradeImage;
        public TextMeshProUGUI upgradeAmountTMP;
        [ReadOnly]
        public Sprite upgradeSprite;
        [HideInInspector]
        public int upgradeAmount;

        [Header("Sell")]
        public Button sellButton;
        public Image sellImage;
        public TextMeshProUGUI sellAmountTMP;
        [HideInInspector]
        public int sellAmount;

        public void SetUpgradeInfo()
        {
            upgradeImage.sprite = upgradeSprite;
            upgradeAmountTMP.text = "$" + upgradeAmount.ToString();
        }

        public void SetSellInfo()
        {
            sellImage.sprite = currentSprite;
            sellAmountTMP.text = "$" + sellAmount.ToString();
        }
    }
}

