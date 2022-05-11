using Objects.Interactable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class IntegratedWeaponSkill : Singleton<IntegratedWeaponSkill>, ISingletonObject
{
    /// <summary>
    /// 플레이어 앞에 무언가 상호작용 가능한 오브젝트가 있다면 false, 없으면 true 리턴
    /// </summary>
    /// <param name="boxCenterOffset">체크할 위치를 offset 시키는 값</param>
    /// <returns></returns>
    public bool CheckForward(Vector3 boxCenterOffset = new Vector3())
    {
        Collider[] cols = Physics.OverlapBox(PlayerBaseTrm.position + PlayerBaseTrm.up * 1.2f + boxCenterOffset,
                                             new Vector3(0.5f, 0.3f, 0.65f),
                                             PlayerBaseTrm.rotation); // 플레이어의 바로 앞을 검사해서 뭔가 있는지 확인

        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].TryGetComponent(out WeaponSkill skill) || cols[i].transform == PlayerBaseTrm || cols[i].isTrigger)
            {
                continue;
            }
            Debug.Log(cols[i].name);
            if (cols[i].TryGetComponent(out Interactable outII)) // 만약 상호작용 되는 오브젝트가 앞에 있었으면 리턴
            {
                // 뭔가에 막힌다 그럼 여기로 옴
                return false;
            }
        }

        return true;
    }


}
