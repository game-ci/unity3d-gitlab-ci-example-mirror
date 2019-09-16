using RPGM.Core;
using RPGM.Gameplay;
using UnityEngine;

namespace RPGM.UI
{
    /// <summary>
    /// Sends user input to the correct control systems.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public float stepSize = 0.1f;
        public Controller mobileController;
        public Controller standaloneController;
        public Canvas mobileCanvas;
        GameModel model = Schedule.GetModel<GameModel>();

        private Controller controller;

        public enum State
        {
            CharacterControl,
            DialogControl,
            Pause
        }

        public void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            controller = mobileController;
#else
            controller = standaloneController;
            mobileCanvas.enabled = false;
#endif
        }

        State state;

        public void ChangeState(State state) => this.state = state;

        void Update()
        {
            switch (state)
            {
                case State.CharacterControl:
                    CharacterControl();
                    break;
                case State.DialogControl:
                    DialogControl();
                    break;
            }
        }

        void DialogControl()
        {
            model.player.nextMoveCommand = Vector3.zero;
            if (controller.GetInputDown(InputButton.Left))
                model.dialog.FocusButton(-1);
            else if (controller.GetInputDown(InputButton.Right))
                model.dialog.FocusButton(+1);
            if (controller.GetInputDown(InputButton.Action))
                model.dialog.SelectActiveButton();
        }

        void CharacterControl()
        {
            if (controller.GetInput(InputButton.Up))
                model.player.nextMoveCommand = Vector3.up * stepSize;
            else if (controller.GetInput(InputButton.Down))
                model.player.nextMoveCommand = Vector3.down * stepSize;
            else if (controller.GetInput(InputButton.Left))
                model.player.nextMoveCommand = Vector3.left * stepSize;
            else if (controller.GetInput(InputButton.Right))
                model.player.nextMoveCommand = Vector3.right * stepSize;
            else
                model.player.nextMoveCommand = Vector3.zero;
        }
    }
}