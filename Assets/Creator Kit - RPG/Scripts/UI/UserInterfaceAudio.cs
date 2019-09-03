using System.Collections.Generic;
using UnityEngine;


namespace RPGM.UI
{

    [RequireComponent(typeof(AudioSource))]
    public class UserInterfaceAudio : MonoBehaviour
    {
        static UserInterfaceAudio instance;

        public AudioClip onButtonClick, onButtonEnter, onButtonExit;
        public AudioClip onShowDialog, onHideDialog;
        public AudioClip onCollect;
        public AudioClip onStoryItem;

        public AudioClip[] vocals;

        AudioSource audioSource;
        AudioClip speech;

        struct SpeechSyllable
        {
            public int index;
            public float time;
        }

        Queue<SpeechSyllable> syllables = new Queue<SpeechSyllable>();

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (instance != null)
                Destroy(instance);
            else
                instance = this;
        }

        void Update()
        {
            if (syllables.Count > 0 && syllables.Peek().time < Time.time)
            {
                var s = syllables.Dequeue();
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                Play(vocals[s.index]);
            }
        }

        void PlaySpeech(int seed, int syllableCount, float pitch)
        {
            Random.InitState(seed);
            var now = Time.time;
            for (var i = 0; i < syllableCount; i++)
            {
                now += Random.Range(0.1f, 0.3f);
                syllables.Enqueue(new SpeechSyllable() { index = Random.Range(0, vocals.Length), time = now });
            }
        }

        public static void Speak(int seed, int syllables, float pitch)
        {
            if (instance != null)
                instance.PlaySpeech(seed, syllables, pitch);
        }

        public static void OnCollect()
        {
            if (instance != null) instance.Play(instance.onCollect);
        }

        public static void OnButtonEnter()
        {
            if (instance != null) instance.Play(instance.onButtonEnter);
        }

        public static void OnButtonExit()
        {
            if (instance != null) instance.Play(instance.onButtonExit);
        }

        internal static void OnStoryItem()
        {
            if (instance != null) instance.Play(instance.onStoryItem);
        }

        public static void OnShowDialog()
        {
            if (instance != null) instance.Play(instance.onShowDialog);
        }

        public static void OnButtonClick()
        {
            if (instance != null) instance.Play(instance.onButtonClick);
        }

        void Play(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public static void OnHideDialog()
        {
            if (instance != null)
                instance.Play(instance.onHideDialog);
        }

        public static void PlayClip(AudioClip audioClip)
        {
            if (instance != null) instance.Play(audioClip);
        }
    }
}