using System;
using UnityEngine;

namespace Characters.Player.Mouse
{

   public class Mouse : MonoBehaviour, IMousedeltaRecvable
   {
      private Transform _camTrm = null;
      private Transform CamTrm
      {
         get
         {
            if (_camTrm == null)
               _camTrm = Camera.main.transform;

            return _camTrm;
         }
      }

      private Transform _playerTrm = null;
      private Transform PlayerTrm
      {
         get
         {
            if (_playerTrm == null)
               _playerTrm = GameObject.FindWithTag("PLAYER_BASE").transform;

            return _playerTrm;
         }
      }


      // For Y rotaion clamping
      public float rotY = 0.0f;


      public void OnMouseX(float x)
      {
         if (!PlayerStatus.Moveable) return;


         // 4원수는 교환 법칙이 성립되지 않는대요
         // 4원수 * 곱하고자 하는 수 = local
         // 곱하고자 하는 수 * 4원수 = world
         PlayerTrm.rotation =
                  PlayerTrm.rotation
                  * Quaternion.Euler(0.0f,
                                     x * PlayerValues.MouseSpeed,
                                     0.0f);
      }

      public void OnMouseY(float y, bool includingMouseSpeed = true)
      {
         if (!PlayerStatus.Moveable) return;
         // 마우스 감도 사용할 지
         rotY -= y * (includingMouseSpeed ? PlayerValues.MouseSpeed : 1.0f);
         rotY = Mathf.Clamp(rotY, -90f, 90f);

         CamTrm.localRotation = Quaternion.Euler(rotY, 0.0f, 0.0f);
      }
   }
}