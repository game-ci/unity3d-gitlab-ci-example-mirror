using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace RPGM.UI
{
    public class SpriteButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public SpriteRenderer spriteRenderer;
        public TextMeshPro textMeshPro;

        public Vector2 Size => spriteRenderer.size;
        public event System.Action onClickEvent;

        public void Enter()
        {
            textMeshPro.color = Color.yellow;
            UserInterfaceAudio.OnButtonEnter();
        }

        public void Exit()
        {
            textMeshPro.color = Color.white;
            UserInterfaceAudio.OnButtonExit();
        }

        public void Click()
        {
            if (onClickEvent != null) onClickEvent();
            textMeshPro.color = Color.white;
            UserInterfaceAudio.OnButtonClick();
        }

        public void OnPointerClick(PointerEventData eventData) => Click();

        public void OnPointerEnter(PointerEventData eventData) => Enter();

        public void OnPointerExit(PointerEventData eventData) => Exit();

        public void SetText(string text) => textMeshPro.text = text;

        void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            textMeshPro = GetComponentInChildren<TextMeshPro>();
        }
    }
}