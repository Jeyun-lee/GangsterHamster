using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons.Actions
{
    public class WeaponEvents : MonoSingleton<WeaponEvents>
    {
        public UnityEvent ChangedOneStep;
        public UnityEvent ChangedTwoStep;
        public UnityEvent ChangedMinSize;
    }
}