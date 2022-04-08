using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Actions
{
    // 이너시오, 그랜드, 그래비토 3가지의 무기들을 관리하기 위한 부모 클래스이며
    // 공통된 기능들을 함수로 정리 해놓기도 하였다.
    public class WeaponAction : MonoBehaviour
    {
        [HideInInspector] public WeaponEnum _weaponEnum; // 상속받은 무기의 종류

        public bool possibleUse = false; // 사용이 가능한가

        // 기본적인 함수들

        /// <summary>
        /// 좌클릭으로 무기 발사
        /// </summary>
        public virtual void FireWeapon()
        {

        }

        /// <summary>
        /// 우클릭으로 능력 발동
        /// </summary>
        public virtual void UseWeapon()
        {

        }

        /// <summary>
        /// R로 무기 회수
        /// </summary>
        public virtual void ResetWeapon()
        {

        }

        public virtual bool IsHandleWeapon()
        {
            return false;
        }

        /// <summary>
        /// 들어온 인자값에 따라 SetActive True, false 해주는 함수
        /// </summary>
        public bool SetActiveWeaponObj(WeaponEnum wenum)
        {
            if (!possibleUse) return false;

            gameObject.SetActive(wenum == _weaponEnum || !IsHandleWeapon());

            return gameObject.activeSelf;
        }


    }
}