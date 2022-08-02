using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class GrandAbilitySound : SoundController
{
    public override void PlaySound(object obj)
    {
        Debug.Log("그랜드 능력 사운드");
        SoundManager.Instance.Play("GrandAbilityExecute");
    }
}
