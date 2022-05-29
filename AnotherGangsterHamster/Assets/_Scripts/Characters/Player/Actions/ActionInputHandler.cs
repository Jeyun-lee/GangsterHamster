using UnityEngine;
using System.Collections.Generic;
using _Core.Commands;


namespace Characters.Player.Actions
{
   [RequireComponent(typeof(Actions))]
   public class ActionInputHandler : MonoBehaviour
   {
      public Transform RightHandTrm = null;
      public string _path = "KeyCodes/Actions.json";

      private Dictionary<KeyCode, Command> _actionDownCommands;
      private Dictionary<KeyCode, Command> _actionUpCommands;

      // IActionable 구체화 한 클레스
      private Actions _actions;

      // 키코드
      private ActionVO _key;

      // 엑션 커멘드
      private DashStart    _dashStart;
      private DashEnd      _dashEnd;
      private CrouchStart  _crouchStart;
      private CrouchEnd    _crouchEnd;
      private Jump         _jump;
      private Interaction  _interaction;

      private void Start()
      {
         Debug.Assert(RightHandTrm != null);

         _actionDownCommands  = new Dictionary<KeyCode, Command>();
         _actionUpCommands    = new Dictionary<KeyCode, Command>();
         _actions             = GetComponent<Actions>();

         _dashStart     = new DashStart(_actions);
         _dashEnd       = new DashEnd(_actions);
         _crouchStart   = new CrouchStart(_actions);
         _crouchEnd     = new CrouchEnd(_actions);
         _jump          = new Jump(_actions);
         _interaction   = new Interaction(_actions);

         RemapCommands();
      }

      public void RemapCommands()
      {
         _actionDownCommands.Clear();

         _key = Utils.JsonToVO<ActionVO>(_path);

         _actionDownCommands.Add((KeyCode)_key.Crouch,     _crouchStart);
         _actionDownCommands.Add((KeyCode)_key.Dash,       _dashStart);
         _actionDownCommands.Add((KeyCode)_key.Jump,       _jump);

         _actionUpCommands.Add((KeyCode)_key.Crouch, _crouchEnd);
         _actionUpCommands.Add((KeyCode)_key.Dash,   _dashEnd);
      }

      private void Update()
      {
         foreach (KeyCode key in _actionDownCommands.Keys)
         {
            if (Input.GetKeyDown(key))
               _actionDownCommands[key].Execute();
         }

         foreach (KeyCode key in _actionUpCommands.Keys)
         {
            if (Input.GetKeyUp(key))
               _actionUpCommands[key].Execute();
         }

         // 상호작용
         if (Input.GetKeyDown((KeyCode)_key.Interact))
         {
            _interaction.Execute(RightHandTrm);
         }
      }

   }
   
}