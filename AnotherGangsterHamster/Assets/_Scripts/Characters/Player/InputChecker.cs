using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Characters.Player
{
   public class InputChecker : MonoBehaviour
   {
      public List<InputCheckObj> inputList
         = new List<InputCheckObj>();

      public UnityEvent OnCompleted;

      [field: SerializeField]
      public bool StepByStepInput { get; set; } = false;

      private int _idx = 0; // Current Input index
      private bool _keyReseted = true;
      private bool _next = true; // can get next input

      private Action<Action> _inputCompareable;

      private Coroutine _stillPressingRoutine;

      private void Awake()
      {
         if (inputList.Count <= 0)
         {
            Logger.Log("InputChecker: List empty. Disabling checker.");
            this.enabled = false;
            return;
         }

         if (StepByStepInput)
         {
            _inputCompareable = callback => {
               if (!_keyReseted) return;

               var input   = inputList[_idx];
               bool result = Input.GetKeyDown(input.key);

               if (result) // 올바른 입력 이뤄질 시
                  ExecuteInputEvent(input, callback);

            }; // _inputCompareable
         } // if (StepByStepInput)

         else // Non-Step by step input check
         {
            _inputCompareable = callback => {
               if (!_keyReseted) return;

               bool result = false;

               for (int i = 0; i < inputList.Count; ++i)
               {
                  if (result) return;

                  var input  = inputList[i];
                      result = Input.GetKeyDown(input.key);

                  if (result) // 올바른 입력 이뤄질 시
                     ExecuteInputEvent(input, callback);

               }; // inputList.ForEach(e => { });
            }; // _inputCompareable

         } // else
      } // Awake();

      private void Update()
      {
         if (!_next) return;

         _inputCompareable(() => {
            Debug.Log("Pressed");

            if (inputList[_idx].OnActionCompleted != null) {
               _next = false;
            }

            inputList[_idx].OnPressed?.Invoke();

            if (++_idx >= inputList.Count) // 완료 시 비활성화
            {
               Logger.Log(
                  $"Check Completed for {inputList.Count} conditions."
               + "Disabling checker.");

               OnCompleted?.Invoke();
               this.enabled = false;
            } // if (_idx >= inputList.Count)
         }); // _inputCompareable();
      }

      IEnumerator CaptureKeyUpEvent(KeyCode key) // 키 리셋 이벤트
      {
         while(!Input.GetKeyUp(key))
            yield return null;

         _keyReseted = true;
      }

      IEnumerator IsStillPressing(KeyCode key, float duration, Action callback)
      {
         _keyReseted = false;

         yield return new WaitForSeconds(duration);
         if (Input.GetKey(key))
            callback();

         _stillPressingRoutine = null;
      }

      private void ExecuteInputEvent(InputCheckObj input, Action callback)
      {

         if (_stillPressingRoutine != null) {
            Debug.Log("A");
            StopCoroutine(_stillPressingRoutine);
         }

         _stillPressingRoutine = StartCoroutine(
            IsStillPressing(input.key, input.duration,
                           () => {
                              callback();
                              StartCoroutine(
                                 CaptureKeyUpEvent(input.key)
                              );
                           }
            )
         ); // StartCoroutine(IsStillPressing());
      }

   }


   [Serializable]
   public class InputCheckObj
   {
      public UnityEvent OnPressed;
      public KeyCode key;

      // 키를 눌러서 발생한 Action 이 종료될 때 호출, 없다면 바로 진행함
      public UnityEvent OnActionCompleted = null;

      public float duration = 0.0f;
   }
}