using Matters.Velocity;
using Objects.Interaction;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Weapons.Actions.Broker;

namespace Weapons.Actions
{
    public enum WeaponEnum
    {
        None,
        Lumo,
        Grand,
        Gravito,
    }
    public class WeaponManagement : MonoBehaviour
    {
        public WeaponEnum startHandleWeapon;

        private Transform _playerBaseTransform;

        private Transform PlayerBaseTransform
        {
            get
            {
                if (_playerBaseTransform == null)
                {
                    _playerBaseTransform = GameObject.FindGameObjectWithTag("PLAYER_BASE").transform;
                }

                return _playerBaseTransform;
            }
        }

        private Image _grandIcon;
        private Image _gravitoIcon;
        private Image _lumoIcon;

        private AllWeaponMessageBroker _messageBroker;

        private Transform grandCharge;
        private Transform lumoCube;
        private Dictionary<WeaponEnum, WeaponAction> _weaponActions = new Dictionary<WeaponEnum, WeaponAction>();
        private Dictionary<WeaponEnum, Image> _weaponIcons = new Dictionary<WeaponEnum, Image>();

        private WeaponEnum _curWeapon = WeaponEnum.None;

        private void Awake()
        {
            _weaponActions.Clear();

            _grandIcon = GameObject.Find("GrandIcon").GetComponent<Image>();
            _gravitoIcon = GameObject.Find("GravitoIcon").GetComponent<Image>();
            _lumoIcon = GameObject.Find("LumoIcon").GetComponent<Image>();

            _messageBroker = GetComponent<AllWeaponMessageBroker>();

            _weaponIcons.Add(WeaponEnum.Grand, _grandIcon);
            _weaponIcons.Add(WeaponEnum.Gravito, _gravitoIcon);
            _weaponIcons.Add(WeaponEnum.Lumo, _lumoIcon);

            _grandIcon.gameObject.SetActive(false);
            _gravitoIcon.gameObject.SetActive(false);
            _lumoIcon.gameObject.SetActive(false);

            _curWeapon = startHandleWeapon;
        }

        private void Start()
        {
            // �ڽ� ������Ʈ�� 1������, 2������, 3������ ������� ��ġ�Ǿ� �ִٴ� �����Ͽ� ������� �ڵ�
            WeaponAction[] childWeaponActions = transform.GetComponentsInChildren<WeaponAction>();

            foreach (WeaponAction weaponAction in childWeaponActions)
            {
                _weaponActions.Add(weaponAction._weaponEnum, weaponAction);

                weaponAction.gameObject.SetActive(startHandleWeapon == weaponAction._weaponEnum);
            }

            grandCharge = GameObject.Find("GrandCharge").transform;
            lumoCube = _weaponActions[WeaponEnum.Lumo].transform.GetChild(0);
            grandCharge.gameObject.SetActive(startHandleWeapon == WeaponEnum.Grand);

            if (startHandleWeapon != WeaponEnum.None)
                _weaponIcons[startHandleWeapon].gameObject.SetActive(true);
        }

        // ��Ŭ�� �� �ߵ��Ǵ� �Լ�
        public void FireCurrentWeapon()
        {
            if(!InteractionManager.Instance.GetGrep())
            {
                if (_curWeapon != WeaponEnum.None)
                {
                    _weaponActions[_curWeapon].FireWeapon();
                }
            }
        }

        // ��Ŭ�� �� �ߵ��Ǵ� �Լ�
        public void UseCurrentWeapon()
        {
            if (_curWeapon != WeaponEnum.None)
            {
                _weaponActions[_curWeapon].UseWeapon();
            }
        }

        // RŰ ������ �ߵ��Ǵ� �Լ�
        public void ResetCurrentWeapon()
        {
            if (_curWeapon != WeaponEnum.None)
            {
                _weaponActions[_curWeapon].ResetWeapon();

                if (_weaponActions[WeaponEnum.Lumo].gameObject.activeSelf)
                    PlayerBaseTransform.GetComponent<FollowGroundPos>().Deactive(lumoCube.gameObject);

                if(_curWeapon == WeaponEnum.Grand)
                {
                    // �׷��忡 ���Ⱑ �پ��ִµ� 
                    // �׷��带 ���� ��Ű�� ���� ���� ��
                    if (_weaponActions[WeaponEnum.Lumo].GetComponent<Lumo>().SticklyTrm() == _weaponActions[WeaponEnum.Grand].transform)
                    {
                        _weaponActions[WeaponEnum.Lumo].ResetWeapon();
                        _weaponActions[WeaponEnum.Lumo].gameObject.SetActive(false);
                    }
                    if(_weaponActions[WeaponEnum.Gravito].GetComponent<Gravito>().SticklyTrm() == _weaponActions[WeaponEnum.Grand].transform)
                    {
                        _weaponActions[WeaponEnum.Gravito].ResetWeapon();
                    }
                }

                if (InteractionManager.Instance.GetGrep())
                {
                    StartCoroutine(Delay(0.05f));
                }
            }
        }

        IEnumerator Delay(float time)
        {
            yield return new WaitForSeconds(time);
            PlayerBaseTransform.GetComponent<FollowGroundPos>().Deactive(_weaponActions[_curWeapon].gameObject);

            if(_curWeapon == WeaponEnum.Gravito)
            {
                yield return new WaitForSeconds((_weaponActions[_curWeapon] as Gravito).gravityChangeTime);
                _weaponActions[_curWeapon].gameObject.SetActive(false);
            }
            else
            {
                _weaponActions[_curWeapon].gameObject.SetActive(false);
            }
        }

        // 1,2,3�� ���� ���� ������ �ߵ��Ǵ� �Լ�
        public void ChangeCurrentWeapon(WeaponEnum weaponEnum)
        {
            foreach (WeaponAction weaponAction in _weaponActions.Values)
            {
                if (!weaponAction.possibleUse)
                    continue;

                _messageBroker.OnChangeWeapon?.Invoke();

                if (weaponEnum == weaponAction._weaponEnum && weaponAction.possibleUse)
                {
                    if (_curWeapon != WeaponEnum.None)
                        _weaponIcons[_curWeapon].gameObject.SetActive(false);

                    _curWeapon = weaponEnum;
                    _weaponIcons[_curWeapon].gameObject.SetActive(true);
                    grandCharge.gameObject.SetActive(_curWeapon == WeaponEnum.Grand && _weaponActions[_curWeapon].possibleUse);
                }

                weaponAction.SetActiveWeaponObj(weaponEnum);
            }
        }

        public WeaponEnum GetCurrentWeapon()
        {
            return _curWeapon;
        }

        public WeaponAction GetCurrentWeaponAction()
        {
            if (_curWeapon == WeaponEnum.None || !_weaponActions.ContainsKey(_curWeapon) || !_weaponActions[_curWeapon].possibleUse) return null;

            return _weaponActions[_curWeapon];
        }
    }
}
