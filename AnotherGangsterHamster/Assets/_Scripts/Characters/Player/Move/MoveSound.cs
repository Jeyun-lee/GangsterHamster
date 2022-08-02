using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

namespace Characters.Player.Move 
{
    public class MoveSound : SoundController
    {
        private MoveInputHandler _moveInputHandler;

        private void Start()
        {
            _moveInputHandler = GetComponent<MoveInputHandler>();
        }

        public override void PlaySound(object obj)
        {

        }
    }
}


