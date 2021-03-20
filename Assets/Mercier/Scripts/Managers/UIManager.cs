using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mercier.Scripts.Classes.Abstract.Turret;
using TMPro;
using Mercier.Scripts.Classes.Custom.UI;

namespace Mercier.Scripts.Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private List<Image> _uIImageOverlay = new List<Image>();
        [SerializeField]
        private Color[] _uIStatusColors =
        {
            new Color32(85, 198, 244, 255), // good
            new Color32(244, 231, 85, 255), // fair
            new Color32(244, 87, 85, 255), // danger
            Color.white // destroyed
        };

        [SerializeField]
        private ArmoryUI _armoryUI;

        [SerializeField]
        private TextMeshProUGUI _startTimerTMP;

        public int DecoyToActivate { set { OnDecoyTurretSelectedFromArmory(value); } }
        
        public static event Func<int, Sprite> onRequestSpriteFromDatabase;
        public static event Func<int, int> onRequestCostFromDatabase;
        public static event Action<bool, int> onDecoyTurretSelectedFromArmory;
        public static event Action onUpgradeSelectedTurret;

        private void Start()
        {
            _startTimerTMP.text = "00:00";

            //PopulateArmoryButtons();
        }

        private void OnEnable()
        {
            GameManager.onUpdateCountdownTimer += UpdateStartTimer;
            GameManager.onUpdatePlayerStatus += UpdateOverlayColor;
            GameManager.onUpdateWarFunds += UpdateWarFunds;

            TowerManager.onSelectActiveTurret += ActiveTurretSelected;
        }

        private void OnDisable()
        {
            GameManager.onUpdateCountdownTimer -= UpdateStartTimer;
            GameManager.onUpdatePlayerStatus -= UpdateOverlayColor;
            GameManager.onUpdateWarFunds -= UpdateWarFunds;

            TowerManager.onSelectActiveTurret -= ActiveTurretSelected;
        }

        private void UpdateStartTimer(float min, float sec)
        {
            _startTimerTMP.text = min.ToString("00") + ":" + sec.ToString("00");
        }

        private void UpdateWarFunds(int warFunds)
        {
            _warFundsAmount.text = warFunds.ToString();
        }

        private void UpdateOverlayColor(int playerStatus)
        {
            _uIImageOverlay.ForEach(img => img.color = _uIStatusColors[playerStatus]);
        }

        private void PopulateArmoryButtons()
        {
            for (int i = 0; i < _armoryUI.turretImages.Length; i++)
            {
                //_armoryTurretImages[i].sprite = OnRequestSpriteFromDatabase(i);
                // how to use TryGetComponent on a child?
                _armoryUI.turretImages[i].GetComponentInChildren<TextMeshProUGUI>().text = "$" + OnRequestCostFromDatabase(i).ToString();
            }
        }

        private Sprite OnRequestSpriteFromDatabase(int index)
        {
            return onRequestSpriteFromDatabase?.Invoke(index);
        }

        private int OnRequestCostFromDatabase(int index)
        {
            return (int)onRequestCostFromDatabase?.Invoke(index);
        }

        private void OnDecoyTurretSelectedFromArmory(int index)
        {
            // disable buttons if not enough funds
            // if there are, activate OnDecoyTurretSelected from TowerManager
            // TowerManager.Instance.OnDecoyTurretSelected(true, index);

            onDecoyTurretSelectedFromArmory?.Invoke(true, index);
        }

        private void ActiveTurretSelected(BaseTurret selectedTurret)
        {
            if (selectedTurret.TurretStats.upgradeSprite == null)
            {
                _armoryUI.upgradeButton.interactable = false;
            }
            else
            {
                _armoryUI.upgradeButton.interactable = true;
                _armoryUI.upgradeSprite = selectedTurret.TurretStats.upgradeSprite;
                _armoryUI.upgradeAmount = selectedTurret.TurretStats.UpgradeCost;
            }

            _armoryUI.currentSprite = selectedTurret.TurretStats.currentSprite;
            _armoryUI.sellAmount = selectedTurret.TurretStats.SellAmount;

            _armoryUI.baseMenu.SetActive(true);
        }

        public void TurretToUpgrade()
        {
            _armoryUI.SetUpgradeInfo();
        }

        public void OnUpgradeSelectedTurret()
        {
            onUpgradeSelectedTurret?.Invoke();
        }

        public void TurretToSell()
        {
            _armoryUI.SetSellInfo();
        }



        //*******************************************************************************************************
        


        [Header("War Funds")]
        [SerializeField]
        private Text _warFundsAmount;

        private Ray _ray;
        private RaycastHit _rayHit;

        

        public static event Action<GameObject> onDisableTurret;




        // click on active tower in scene
        //  once turret selected, show upgrade and selling options
        //      if turret not able to be upgraded, have button greyed out and can't click
        //  if sold, increase war funds by 50% of cost
        //  if upgraded
        //      need reference to possition turret is currently at

        

        public void SellTurret()
        {
            //TowerManager.Instance.ActiveTurretList.Remove(_turretToModify);

            //_turretToModify.SetActive(false);

            //OnDisableTurret(_towerPosition);

            //GameManager.Instance.WarFundsAvailable += _sellAmount;
            //_warFundsAmount.text = GameManager.Instance.WarFundsAvailable.ToString();
        }

        private void OnDisableTurret(GameObject turret)
        {
            onDisableTurret?.Invoke(turret);
        }
    }
}

