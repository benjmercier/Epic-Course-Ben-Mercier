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

        [Header("War Funds")]
        [SerializeField]
        private Text _warFundsAmount;

        public int DecoyToActivate { set { OnDecoyTurretSelectedFromArmory(value); } }
        
        public static event Func<int, Sprite> onRequestSpriteFromDatabase;
        public static event Func<int, int> onRequestCostFromDatabase;
        public static event Action<bool, int> onDecoyTurretSelectedFromArmory;
        public static event Action onUpgradeSelectedTurret;
        public static event Action onSellSelectedTurret;
        public static event Action<bool> onModifyTurretMenuDisabled;

        private void Start()
        {
            _startTimerTMP.text = "00:00";
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

            EnableArmoryButtons();
        }

        private void UpdateOverlayColor(int playerStatus)
        {
            _uIImageOverlay.ForEach(img => img.color = _uIStatusColors[playerStatus]);
        }

        private void EnableArmoryButtons()
        {
            for (int i = 0; i < _armoryUI.turretButtons.Length; i++)
            {
                _armoryUI.turretButtons[i].interactable = GameManager.Instance.EnoughWarFundsAvailable(
                    OnRequestCostFromDatabase(i));
            }
        }

        private int OnRequestCostFromDatabase(int index)
        {
            return (int)onRequestCostFromDatabase?.Invoke(index);
        }

        private void OnDecoyTurretSelectedFromArmory(int index)
        {
            onDecoyTurretSelectedFromArmory?.Invoke(true, index);
        }

        public void OnModifyTurretMenuDisabled()
        {
            onModifyTurretMenuDisabled?.Invoke(true);
        }

        private void ActiveTurretSelected(BaseTurret selectedTurret)
        {
            if (!selectedTurret.TurretStats.isUpgradeable)
            {
                _armoryUI.upgradeButton.interactable = false;
            }
            else
            {
                if (!GameManager.Instance.EnoughWarFundsAvailable(selectedTurret.TurretStats.UpgradeCost))
                {
                    _armoryUI.upgradeButton.interactable = false;
                }
                else
                {
                    _armoryUI.upgradeButton.interactable = true;
                    _armoryUI.upgradeSprite = selectedTurret.TurretStats.upgradeTo.TurretStats.currentSprite;
                    _armoryUI.upgradeAmount = selectedTurret.TurretStats.UpgradeCost;
                }
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

        public void OnSellSelectedTurret()
        {
            onSellSelectedTurret?.Invoke();
        }

        public void Play()
        {
            GameManager.Instance.TransitionToState(GameManager.GameState.Playing);
        }

        public void Pause()
        {
            GameManager.Instance.TransitionToState(GameManager.GameState.Paused);
        }

        public void FastForward()
        {
            GameManager.Instance.TransitionToState(GameManager.GameState.FastForward);
        }
    }
}

