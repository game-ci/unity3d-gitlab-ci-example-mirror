using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace RPGM.UI
{
    [ExecuteInEditMode]
    public class SpriteUIElement : MonoBehaviour
    {
        new public Camera camera;

        public int pixelsPerUnit = 64;

        public Vector2 anchor;
        public Vector2 offset;

        public Vector2 hideOffset;

        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        public float animationDuration = 0.5f;

        SpriteRenderer spriteRenderer;
        PixelPerfectCamera pixelPerfectCamera;
        Vector2 animationOffset;

        float t = 0;
        float direction = 0;

        [ContextMenu("Show")]
        public void Show()
        {
            direction = 1;
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            direction = -1;
        }

        public void Toggle()
        {
            direction *= -1;
        }

        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (Application.isPlaying)
                pixelPerfectCamera = camera.GetComponent<PixelPerfectCamera>();
            anchor.x = Mathf.Round(anchor.x * pixelsPerUnit) / pixelsPerUnit;
            anchor.y = Mathf.Round(anchor.y * pixelsPerUnit) / pixelsPerUnit;
        }

        void Update()
        {
            if (camera != null)
            {
                t = Mathf.Clamp01(t + (direction * Time.deltaTime / animationDuration));

                animationOffset = Vector2.LerpUnclamped(hideOffset, Vector3.zero, curve.Evaluate(t));
                var p = (Vector2)camera.ViewportToWorldPoint(anchor + offset + animationOffset);
                transform.position = p;
                if (pixelPerfectCamera != null && Application.isPlaying)
                {
                    transform.position = pixelPerfectCamera.RoundToPixel(transform.position);
                }
            }
        }
    }
}