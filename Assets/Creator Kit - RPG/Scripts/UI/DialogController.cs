using System;
using RPGM.Core;
using RPGM.Gameplay;
using UnityEngine;


namespace RPGM.UI
{
    public class DialogController : MonoBehaviour
    {
        public DialogLayout dialogLayout;

        public System.Action<int> onButton;

        public int selectedButton = 0;
        public int buttonCount = 0;

        SpriteButton[] buttons;
        Camera mainCamera;
        GameModel model = Schedule.GetModel<GameModel>();
        SpriteUIElement spriteUIElement;
        public void FocusButton(int direction)
        {
            if (buttonCount > 0)
            {
                if (selectedButton < 0) selectedButton = 0;
                buttons[selectedButton].Exit();
                selectedButton += direction;
                selectedButton = Mathf.Clamp(selectedButton, 0, buttonCount - 1);
                buttons[selectedButton].Enter();
            }
        }

        public void SelectActiveButton()
        {
            if (buttonCount > 0)
            {
                if (selectedButton >= 0)
                {
                    model.input.ChangeState(InputController.State.CharacterControl);
                    buttons[selectedButton].Click();
                    selectedButton = -1;
                }
            }
            else
            {
                //there are no buttons, just Hide when required.
                model.input.ChangeState(InputController.State.CharacterControl);
                Hide();
            }
        }

        public void Show(SpriteRenderer contextSprite, string text)
        {
            var position = contextSprite.transform.position;
            position.x -= contextSprite.size.x;
            position.y -= contextSprite.size.y * 0.5f;
            position.y += dialogLayout.spriteRenderer.size.y;
            Show(position, text);
        }

        public void SetButton(int index, string text)
        {
            var d = dialogLayout;
            d.SetButtonText(index, text);
            buttonCount = Mathf.Max(buttonCount, index + 1);
        }

        public void Show(Vector3 position, string text)
        {
            var d = dialogLayout;
            d.gameObject.SetActive(true);
            d.SetText(text);
            SetPosition(position);
            model.input.ChangeState(InputController.State.DialogControl);
            buttonCount = 0;
            selectedButton = -1;
        }

        public void Show(Vector3 position, string text, string buttonA)
        {
            UserInterfaceAudio.OnShowDialog();
            var d = dialogLayout;
            d.gameObject.SetActive(true);
            d.SetText(text, buttonA);
            SetPosition(position);
            model.input.ChangeState(InputController.State.DialogControl);
            buttonCount = 1;
            selectedButton = -1;
        }

        public void Show(Vector3 position, string text, string buttonA, string buttonB)
        {
            UserInterfaceAudio.OnShowDialog();
            var d = dialogLayout;
            d.gameObject.SetActive(true);
            d.SetText(text, buttonA, buttonB);
            SetPosition(position);
            model.input.ChangeState(InputController.State.DialogControl);
            buttonCount = 2;
            selectedButton = -1;
        }

        void SetPosition(Vector3 position)
        {
            var screenPoint = mainCamera.WorldToScreenPoint(position);
            position = spriteUIElement.camera.ScreenToViewportPoint(screenPoint);
            spriteUIElement.anchor = position;
        }

        public void Show(Vector3 position, string text, string buttonA, string buttonB, string buttonC)
        {
            UserInterfaceAudio.OnShowDialog();
            var d = dialogLayout;
            d.gameObject.SetActive(true);
            d.SetText(text, buttonA, buttonB, buttonC);
            SetPosition(position);
            model.input.ChangeState(InputController.State.DialogControl);
            buttonCount = 3;
            selectedButton = -1;
        }

        public void Hide()
        {
            UserInterfaceAudio.OnHideDialog();
            dialogLayout.gameObject.SetActive(false);
        }

        public void SetIcon(Sprite icon) => dialogLayout.SetIcon(icon);

        void OnButton(int index)
        {
            if (onButton != null) onButton(index);
            onButton = null;
        }

        void Awake()
        {
            dialogLayout.gameObject.SetActive(false);
            buttons = dialogLayout.buttons;
            dialogLayout.buttonA.onClickEvent += () => OnButton(0);
            dialogLayout.buttonB.onClickEvent += () => OnButton(1);
            dialogLayout.buttonC.onClickEvent += () => OnButton(2);
            spriteUIElement = GetComponent<SpriteUIElement>();
            mainCamera = Camera.main;
        }
    }
}