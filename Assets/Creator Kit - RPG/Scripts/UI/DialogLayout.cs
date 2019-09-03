using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace RPGM.UI
{

    [ExecuteInEditMode]
    public class DialogLayout : MonoBehaviour
    {
        public float padding = 0.25f;
        public SpriteRenderer iconRenderer;
        public SpriteRenderer spriteRenderer;
        public TextMeshPro textMeshPro;

        public SpriteButton buttonA, buttonB, buttonC;

        [NonSerialized] public SpriteButton[] buttons;
        Vector2 minSize;

        void Awake()
        {
            buttons = new[] { buttonA, buttonB, buttonC };
            minSize = spriteRenderer.size;
        }

        void OnClick()
        {

        }

        public void SetIcon(Sprite icon)
        {
            if (icon == null)
                iconRenderer.enabled = false;
            else
            {
                iconRenderer.sprite = icon;
                iconRenderer.enabled = true;
            }
        }

        public void SetText(string text)
        {
            SetDialogText(text);
            buttonA.gameObject.SetActive(false);
            buttonB.gameObject.SetActive(false);
            buttonC.gameObject.SetActive(false);
        }

        public void SetButtonText(int index, string text)
        {
            buttons[index].SetText(text);
            buttons[index].gameObject.SetActive(true);
        }

        public void SetText(string text, string buttonAText)
        {
            SetDialogText(text);
            buttonA.gameObject.SetActive(true);
            buttonA.SetText(buttonAText);
            buttonB.gameObject.SetActive(false);
            buttonC.gameObject.SetActive(false);
        }

        public void SetText(string text, string buttonAText, string buttonBText)
        {
            SetDialogText(text);
            buttonA.gameObject.SetActive(true);
            buttonA.SetText(buttonAText);
            buttonB.gameObject.SetActive(true);
            buttonB.SetText(buttonBText);
            buttonC.gameObject.SetActive(false);
        }

        public void SetText(string text, string buttonAText, string buttonBText, string buttonCText)
        {
            SetDialogText(text);
            buttonA.gameObject.SetActive(true);
            buttonA.SetText(buttonAText);
            buttonB.gameObject.SetActive(true);
            buttonB.SetText(buttonBText);
            buttonC.gameObject.SetActive(true);
            buttonC.SetText(buttonCText);
        }


        void SetDialogText(string text)
        {
            textMeshPro.text = text;
            ScaleBackgroundToFitText();
        }

        void Update()
        {
            PositionIcon();
            ScaleBackgroundToFitText();
            PositionButtons();
        }

        void PositionIcon()
        {
            if (iconRenderer.sprite != null)
            {
                var p = new Vector3(1, 0, 0);
                // p.x -= iconRenderer.size.x * 0.5f;
                iconRenderer.transform.localPosition = p;
            }
        }

        void PositionButtons()
        {
            var s = (Vector3)spriteRenderer.size;
            buttonA.transform.localPosition = new Vector3(-0.1f - buttonA.Size.x * 0.5f, (-s.y * 0.5f) - buttonA.Size.y * 0.5f - 0.05f, 0);
            buttonB.transform.localPosition = new Vector3(+0.1f + buttonB.Size.x * 0.5f, (-s.y * 0.5f) - buttonB.Size.y * 0.5f - 0.05f, 0);
            buttonC.transform.localPosition = new Vector3(0, (-s.y * 0.5f) - buttonC.Size.y * 0.5f - 0.1f - buttonC.Size.y, 0);
        }

        void ScaleBackgroundToFitText()
        {
            var s = (Vector2)textMeshPro.bounds.size;
            s += Vector2.one * padding;
            s.x = Mathf.Max(minSize.x, s.x);
            s.y = Mathf.Max(minSize.y, s.y);
            spriteRenderer.size = s;
        }

        public float GetHeight()
        {
            return spriteRenderer.size.y;
        }
    }
}