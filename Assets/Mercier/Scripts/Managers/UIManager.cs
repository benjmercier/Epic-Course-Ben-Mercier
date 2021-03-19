using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mercier.Scripts.Classes.Abstract.Turret;
using TMPro;

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
        private TextMeshProUGUI _startTimerTMP;

        [SerializeField]
        private Image[] _armoryTurretImages;

        public static event Action<int> onTurretSelectedFromArmory;
        public static event Func<int, Sprite> onRequestSprietFromDatabase;
        public static event Func<int, int> onRequestCostFromDatabase;








        [SerializeField]
        private GameObject _turretToModify;
        private Turret _currentTurret;
        [SerializeField]
        private GameObject _towerPosition;

        [SerializeField]
        private GameObject _modifyTurretMenu;

        

        [SerializeField]
        private List<Sprite> _turretSprites = new List<Sprite>();
        private Sprite _currentSprite;
        private Sprite _upgradeSprite;

        [Header("War Funds")]
        [SerializeField]
        private Text _warFundsAmount;

        [Header("Upgrade Turret")]
        [SerializeField]
        private Button _upgradeButton;
        [SerializeField]
        private Image _availableUpgrade;
        [SerializeField]
        private TextMeshProUGUI _amountToUpgrade = new TextMeshProUGUI();
        private int _upgradeAmount;

        [Header("Sell Turret")]
        [SerializeField]
        private Button _sellButton;
        [SerializeField]
        private Image _availableToSell;
        [SerializeField]
        private TextMeshProUGUI _amountToSell = new TextMeshProUGUI();
        private int _sellAmount;

        private Ray _ray;
        private RaycastHit _rayHit;

        public int DecoyToActivate { set { OnTurretSelectedFromArmory(value); } }

        public static event Action<GameObject> onDisableTurret;


        /// <summary>
        /// base UI blue color = 85, 198, 244, 255
        /// </summary>

        private void Start()
        {
            _startTimerTMP.text = "00:00";
        }

        private void OnEnable()
        {
            GameManager.onUpdateCountdownTimer += UpdateStartTimer;
            GameManager.onUpdatePlayerStatus += UpdateOverlayColor;
            GameManager.onUpdateWarFunds += UpdateWarFunds;
        }

        private void OnDisable()
        {
            GameManager.onUpdateCountdownTimer -= UpdateStartTimer;
            GameManager.onUpdatePlayerStatus -= UpdateOverlayColor;
            GameManager.onUpdateWarFunds -= UpdateWarFunds;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PopulateArmoryButtons();
            }
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

        private void OnTurretSelectedFromArmory(int index)
        {
            // disable buttons if not enough funds
            // if there are, activate OnDecoyTurretSelected from TowerManager

        }

        private void PopulateArmoryButtons()
        {
            // check with Database
            // foreach element in Database
            // populate button picture
            
            for (int i = 0; i < _armoryTurretImages.Length; i++)
            {
                _armoryTurretImages[i].sprite = OnRequestSprietFromDatabase(i);
                _armoryTurretImages[i].GetComponentInChildren<TextMeshProUGUI>().text = "$" + OnRequestCostFromDatabase(i).ToString();
            }
        }

        private Sprite OnRequestSprietFromDatabase(int index)
        {
            return onRequestSprietFromDatabase?.Invoke(index);
        }

        private int OnRequestCostFromDatabase(int index)
        {
            return (int)onRequestCostFromDatabase?.Invoke(index);
        }





        private void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectActiveTurret();
            }
        }




        private void ActivateDecoyFromTowerManager(int index)
        {
            TowerManager.Instance.OnDecoyTurretSelected(true, index);
        }


        // click on active tower in scene
        //  once turret selected, show upgrade and selling options
        //      if turret not able to be upgraded, have button greyed out and can't click
        //  if sold, increase war funds by 50% of cost
        //  if upgraded
        //      need reference to possition turret is currently at

        private void SelectActiveTurret()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(_ray, out _rayHit, Mathf.Infinity, GameManager.Instance.TurretLayer))
            {
                Debug.Log("Hit Info: " + _rayHit.transform.name);

                if (TowerManager.Instance.ActiveTurretList.Contains(_rayHit.transform.gameObject))
                {
                    _turretToModify = _rayHit.transform.gameObject;

                    _currentTurret = _turretToModify.GetComponent<Turret>();

                    if (_currentTurret.turretID == 1 || _currentTurret.turretID == 3)
                    {
                        _upgradeButton.interactable = false;
                    }
                    else
                    {
                        _upgradeButton.interactable = true;

                        if (_currentTurret.turretID == 0)
                        {
                            _upgradeSprite = _turretSprites[1];
                        }
                        else
                        {
                            _upgradeSprite = _turretSprites[3];
                        }
                    }

                    _modifyTurretMenu.SetActive(true);

                    _currentSprite = _turretSprites[_currentTurret.turretID];
                    _upgradeAmount = _currentTurret.turretCost;
                    _sellAmount = _upgradeAmount / 2;
                }

                if (Physics.Raycast(_ray, out _rayHit, Mathf.Infinity, GameManager.Instance.TurretPosLayer))
                {
                    _towerPosition = _rayHit.transform.gameObject;
                }
            }
        }

        public void TurretToUpgrade()
        {
            _availableUpgrade.sprite = _upgradeSprite;
            _amountToUpgrade.text = "$" + _upgradeAmount.ToString();
        }

        public void UpgradeTurret()
        {
            // remove turretToModify from active turret list
            // 
            TowerManager.Instance.ActiveTurretList.Remove(_turretToModify);

            _turretToModify.SetActive(false);

            var _newTurret = PoolManager.Instance.ReturnPrefabFromPool(false, 1, _currentTurret.turretID + 1);
            _newTurret.transform.position = _towerPosition.transform.position;
            _newTurret.transform.rotation = _towerPosition.transform.rotation;

            TowerManager.Instance.ActiveTurretList.Add(_newTurret);

            _newTurret.SetActive(true);

            GameManager.Instance.WarFundsAvailable -= _upgradeAmount;

            _warFundsAmount.text = GameManager.Instance.WarFundsAvailable.ToString();
        }

        public void TurretToSell()
        {
            _availableToSell.sprite = _currentSprite;
            _amountToSell.text = "$" + _sellAmount.ToString();
        }

        public void SellTurret()
        {
            TowerManager.Instance.ActiveTurretList.Remove(_turretToModify);

            _turretToModify.SetActive(false);

            OnDisableTurret(_towerPosition);

            GameManager.Instance.WarFundsAvailable += _sellAmount;
            _warFundsAmount.text = GameManager.Instance.WarFundsAvailable.ToString();
        }

        private void OnDisableTurret(GameObject turret)
        {
            onDisableTurret?.Invoke(turret);
        }
    }
}

