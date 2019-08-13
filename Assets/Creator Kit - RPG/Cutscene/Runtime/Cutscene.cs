using System;
using System.Collections;
using System.Collections.Generic;
using RPGM.Core;
using RPGM.UI;
using TMPro;
using UnityEngine;

namespace RPGM.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    [ExecuteInEditMode]
    public class Cutscene : MonoBehaviour
    {
        [System.Serializable]
        public struct CutsceneEvent
        {
            public Texture2D newImage;
            public Texture2D transitionGradient;
            public AudioClip audioClip;
            [Multiline]
            public string newText;
            public float duration;
            public Vector2 pan;
            public float zoom;
        }

        public float zoom = 1f;
        public float fadeInTime = 1;
        public float fadeOutTime = 1;
        public float crossFadeTime = 3;
        public Vector2 pan = Vector2.zero;
        public AudioClip audioClip;
        public bool destroyWhenFinished = true;
        public CutsceneEvent[] cutsceneEvents;

        TextMeshPro textMeshPro;
        Camera mainCamera;
        Material material;
        bool isPlaying = false;
        float textAlpha = 0;
        float textAlphaVelocity;
        Texture2D game;

        Transform image;

        public System.Action<Cutscene> OnFinish;
        GameModel model = Schedule.GetModel<GameModel>();

        void OnEnable()
        {
            mainCamera = Camera.main;
            image = transform.GetChild(0);
            material = image.GetComponent<MeshRenderer>().sharedMaterial;
            textMeshPro = transform.GetChild(1).GetComponent<TextMeshPro>();
            if (Application.isPlaying)
            {
                MatchScreenSize();
                Cutscene.Play(this);
            }
        }

        void Update()
        {
            MatchScreenSize();
            if (Application.isPlaying && isPlaying)
            {
                var c = textMeshPro.color;
                c.a = Mathf.SmoothDamp(c.a, textAlpha, ref textAlphaVelocity, 0.25f, float.MaxValue);
                textMeshPro.color = c;
            }
        }

        void MatchScreenSize()
        {
            transform.position = mainCamera.transform.position + mainCamera.transform.forward;
            float worldScreenHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            image.localScale = new Vector3(worldScreenWidth, worldScreenHeight, 1);
        }

        public static void Play(Cutscene cs)
        {
            if (!cs.isPlaying)
            {
                cs.game = ScreenCapture.CaptureScreenshotAsTexture();
                cs.gameObject.SetActive(true);
                cs.isPlaying = true;
                cs.StartCoroutine(cs._Play());
            }
        }

        IEnumerator _Play()
        {

            if (audioClip != null)
                model.musicController.CrossFade(audioClip);

            material.SetTexture("_ScreenTex", game);

            material.SetTexture("_FrontTex", cutsceneEvents[0].newImage);
            material.SetTexture("_BackTex", game);
            material.SetFloat("_Zoom", zoom);
            material.SetFloat("_PanX", pan.x);
            material.SetFloat("_PanY", pan.y);
            material.SetFloat("_Fade", 0);
            textAlpha = 0;
            var t = 0f;
            while (t <= 1)
            {
                t += Time.deltaTime / fadeInTime;
                material.SetFloat("_Alpha", Mathf.Clamp01(Mathf.SmoothStep(0, 1, t)));
                yield return null;
            }

            for (var i = 0; i < cutsceneEvents.Length; i++)
            {
                var e = cutsceneEvents[i];
                t = 0f;
                var fade = 0f;
                material.SetTexture("_FrontTex", e.newImage);
                material.SetTexture("_NoiseTex", e.transitionGradient);
                material.SetFloat("_Fade", fade);
                textMeshPro.text = e.newText;
                textAlpha = 1;
                var startZoom = zoom;
                var endZoom = e.zoom;
                var startPan = pan;
                var endPan = e.pan;
                if (e.audioClip != null)
                    UserInterfaceAudio.PlayClip(e.audioClip);
                while (t <= 1f)
                {
                    t = Mathf.Clamp01(t);
                    var d = Mathf.SmoothStep(0, 1, t);
                    zoom = Mathf.Lerp(startZoom, endZoom, d);
                    pan = Vector2.Lerp(startPan, endPan, d);
                    material.SetFloat("_Zoom", zoom);
                    material.SetFloat("_PanX", pan.x);
                    material.SetFloat("_PanY", pan.y);
                    t += Time.deltaTime / e.duration;
                    yield return null;
                }
                textAlpha = 0;
                if (i < cutsceneEvents.Length - 1)
                {
                    var next = cutsceneEvents[i + 1];
                    material.SetTexture("_BackTex", next.newImage);
                }
                else
                {
                    material.SetTexture("_BackTex", null);
                }
                fade = 0f;
                while (fade <= 1)
                {
                    fade += Time.deltaTime / crossFadeTime;
                    material.SetFloat("_Fade", fade);
                    yield return null;
                }
            }
            t = 1f;
            while (t > 0)
            {
                t -= Time.deltaTime / fadeOutTime;
                material.SetFloat("_Alpha", Mathf.Clamp01(Mathf.SmoothStep(0, 1, t)));
                yield return null;
            }
            yield return null;
            isPlaying = false;
            gameObject.SetActive(false);
            if (OnFinish != null) OnFinish(this);
            if (Application.isPlaying && destroyWhenFinished)
                Destroy(gameObject);
        }

    }
}