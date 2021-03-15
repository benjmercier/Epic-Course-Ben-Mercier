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
        private GameObject _turretToModify;

        [SerializeField]
        private GameObject _modifyTurretMenu;

        [SerializeField]
        private List<Button> _armoryButtons = new List<Button>();

        [SerializeField]
        private List<Sprite> _turretSprites = new List<Sprite>();
        private Sprite _currentSprite;

        [Header("Upgrade Turret")]
        [SerializeField]
        private Image _availableUpgrade;
        [SerializeField]
        private TextMeshProUGUI _amountToUpgrade = new TextMeshProUGUI();
        private int _upgradeAmount;

        [Header("Sell Turret")]
        [SerializeField]
        private Image _availableToSell;
        [SerializeField]
        private TextMeshProUGUI _amountToSell = new TextMeshProUGUI();
        private int _sellAmount;

        private Ray _ray;
        private RaycastHit _rayHit;

        public int DecoyToActivate { set { ActivateDecoyFromTowerManager(value); } }

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

                    _modifyTurretMenu.SetActive(true);
                    
                    Turret obj = _turretToModify.GetComponent<Turret>();

                    _currentSprite = _turretSprites[obj.turretID];
                    _upgradeAmount = obj.turretCost;
                    _sellAmount = _upgradeAmount / 2;
                }
            }
        }

        public void UpgradeSelectedTurret()
        { 

        }

        public void SellSelectedTurret()
        {
            _availableToSell.sprite = _currentSprite;
            _amountToSell.text = "$" + _sellAmount.ToString();
        }
    }
}

