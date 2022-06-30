using Characters.Player;
using Matters.Gravity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Actions
{
    public class LumoCube : MonoBehaviour
    {
        private bool _isReinforcemented = false;
        private GravityAffectedObject _playerGravity;
        private Rigidbody _playerRigid;
        

        private void Awake()
        {
            _playerGravity = GameObject.FindGameObjectWithTag("PLAYER_BASE").GetComponent<GravityAffectedObject>();
            _playerRigid = _playerGravity.GetComponent<Rigidbody>();
        }

        public void ObjTriggerStayEvent(GameObject obj)
        {
            if (!obj.CompareTag("PLAYER_BASE")) return;

            if (!_isReinforcemented)
            {
                _playerGravity.AffectedByGlobalGravity = false;
                _playerGravity.SetIndividualGravity(GravityManager.GetGlobalGravityDirection(), 4.9f);
                _playerRigid.mass = 2;
                _isReinforcemented = true;
            }
            else if(_playerGravity.AffectedByGlobalGravity)
            {
                _playerGravity.AffectedByGlobalGravity = false;
                _playerGravity.SetIndividualGravity(GravityManager.GetGlobalGravityDirection(), 4.9f);
            }

            PlayerValues.Speed = PlayerValues.DashSpeed * 2;
        }

        public void ObjTriggerExitEvent(GameObject obj)
        {
            if (!obj.CompareTag("PLAYER_BASE")) return;

            if (_isReinforcemented)
            {
                _playerGravity.AffectedByGlobalGravity = true;
                _isReinforcemented = false;
                _playerRigid.mass = 1;
            }
        }

        private void OnDisable()
        {
            _playerGravity.AffectedByGlobalGravity = true;
            _isReinforcemented = false;
        }
    }
}