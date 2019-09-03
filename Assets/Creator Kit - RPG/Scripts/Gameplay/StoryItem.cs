using System.Collections.Generic;
using System.Linq;
using RPGM.Core;
using RPGM.Gameplay;
using RPGM.UI;
using UnityEngine;


namespace RPGM.Gameplay
{
    /// <summary>
    /// This class will trigger a text message when the player enters the trigger,
    /// and optionally start a cutscene.
    /// </summary>
    public class StoryItem : MonoBehaviour, ISerializationCallbackReceiver
    {
        public string ID;
        [Multiline]
        public string text = "There is no story to be found here.";
        public AudioClip audioClip;

        public bool disableWhenDiscovered = false;

        public HashSet<StoryItem> requiredStoryItems;
        public HashSet<InventoryItem> requiredInventoryItems;
        public Cutscene cutscenePrefab;

        [System.NonSerialized] public HashSet<StoryItem> dependentStoryItems = new HashSet<StoryItem>();

        [SerializeField] StoryItem[] _requiredStoryItems;
        [SerializeField] InventoryItem[] _requiredInventoryItems;

        GameModel model = Schedule.GetModel<GameModel>();


        void OnEnable()
        {
            if (ID == string.Empty && text != null)
            {
                ID = $"SI:{text.GetHashCode()}";
            }
        }

        void Awake()
        {
            ConnectRelations();
        }

        public void ConnectRelations()
        {
            foreach (var i in requiredStoryItems)
            {
                i.dependentStoryItems.Add(this);
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (!Application.isPlaying) return;

            foreach (var requiredInventoryItem in requiredInventoryItems)
                if (requiredInventoryItem != null)
                    if (!model.HasInventoryItem(requiredInventoryItem.name))
                        return;
            foreach (var requiredStoryItem in requiredStoryItems)
                if (requiredStoryItem != null)
                    if (!model.HasSeenStoryItem(requiredStoryItem.ID))
                        return;
            if (text != string.Empty)
                MessageBar.Show(text);
            if (ID != string.Empty)
                model.RegisterStoryItem(ID);
            if (audioClip == null)
                UserInterfaceAudio.OnStoryItem();
            else
                UserInterfaceAudio.PlayClip(audioClip);
            if (disableWhenDiscovered) gameObject.SetActive(false);
            if (cutscenePrefab != null)
            {
                var cs = Instantiate(cutscenePrefab);
                if (cs.audioClip != null)
                {
                    cs.OnFinish += (i) => model.musicController.CrossFade(model.musicController.audioClip);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            _requiredInventoryItems = requiredInventoryItems.ToArray();

            _requiredStoryItems = requiredStoryItems.ToArray();
        }

        public void OnAfterDeserialize()
        {
            requiredStoryItems = new HashSet<StoryItem>();
            if (_requiredStoryItems != null)
                foreach (var i in _requiredStoryItems) requiredStoryItems.Add(i);


            requiredInventoryItems = new HashSet<InventoryItem>();
            if (_requiredInventoryItems != null)
                foreach (var i in _requiredInventoryItems) requiredInventoryItems.Add(i);
        }
    }
}