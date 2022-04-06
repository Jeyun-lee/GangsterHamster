using Objects.StageObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Actions
{
    [RequireComponent(typeof(CollisionInteractableObject))]
    public class Grand : WeaponAction
    {
        private void Awake()
        {
            _weaponEnum = WeaponEnum.Grand;
        }
    }
}