using System.Collections.Generic;
using RPGM.Core;
using RPGM.UI;
using UnityEngine;

namespace RPGM.Gameplay
{
    /// <summary>
    /// This class implements quests.
    /// </summary>
    public class Quest : MonoBehaviour
    {
        public enum SpawnMode
        {
            CloneAndEnable,
            CloneOnly
        }

        [System.Serializable]
        public class ItemRequirement
        {
            public InventoryItem item;
            public int count = 1;
        }

        public string title;
        public string desc;
        public ConversationScript questInProgressConversation, questCompletedConversation;

        public SpawnMode spawnMode = SpawnMode.CloneAndEnable;
        bool disableItemsOnStart = true;

        public GameObject[] enableOnQuestStart;
        public GameObject[] spawnOnQuestStart;
        public ItemRequirement[] requiredItems;
        public GameObject[] spawnOnQuestComplete;
        public InventoryItem[] rewardItems;

        public bool destroySpawnsOnQuestComplete = true;

        public Cutscene introCutscenePrefab, outroCutscenePrefab;

        List<GameObject> cleanup = new List<GameObject>();

        public bool isStarted = false;
        public bool isFinished = false;

        GameModel model = Schedule.GetModel<GameModel>();

        void Awake()
        {
            //if required, make sure that items that will be enabled by this quest are disabled
            if (disableItemsOnStart)
            {
                if (enableOnQuestStart != null)
                    foreach (var i in enableOnQuestStart)
                        if (i != null)
                            i.SetActive(false);

                switch (spawnMode)
                {
                    case SpawnMode.CloneAndEnable:
                        foreach (var i in spawnOnQuestStart)
                        {
                            i.SetActive(false);
                        }
                        break;
                }
            }
        }

        public void OnStartQuest()
        {
            isFinished = false;
            if (introCutscenePrefab != null)
            {
                var cs = Instantiate(introCutscenePrefab);
                if (cs.audioClip != null)
                {
                    cs.OnFinish += (i) => model.musicController.CrossFade(model.musicController.audioClip);
                }
            }
            if (enableOnQuestStart != null)
                foreach (var i in enableOnQuestStart)
                    if (i != null)
                        i.SetActive(true);
            switch (spawnMode)
            {
                case SpawnMode.CloneAndEnable:
                    foreach (var i in spawnOnQuestStart)
                    {
                        var clone = GameObject.Instantiate(i);
                        clone.SetActive(true);
                        if (destroySpawnsOnQuestComplete) cleanup.Add(clone);
                    }
                    break;
                case SpawnMode.CloneOnly:
                    foreach (var i in spawnOnQuestStart)
                    {
                        var clone = GameObject.Instantiate(i);
                        if (destroySpawnsOnQuestComplete) cleanup.Add(clone);
                    }
                    break;
            }

        }

        public bool IsQuestComplete()
        {
            var inv = new HashSet<string>(model.InventoryItems);
            foreach (var i in requiredItems)
            {
                if (inv.Contains(i.item.name) && model.GetInventoryCount(i.item.name) >= i.count) continue;
                return false;
            }
            return true;
        }

        public void RewardItemsToPlayer()
        {
            foreach (var i in rewardItems)
            {
                MessageBar.Show($"You collected: {i.name} x {i.count}");
                model.AddInventoryItem(i);
                UserInterfaceAudio.OnCollect();
                i.gameObject.SetActive(false);
            }
            if (outroCutscenePrefab != null)
            {
                var cs = Instantiate(outroCutscenePrefab);
                if (cs.audioClip != null)
                {
                    cs.OnFinish += (i) => model.musicController.CrossFade(model.musicController.audioClip);
                }
            }

        }

        public void OnFinishQuest()
        {
            if (destroySpawnsOnQuestComplete)
            {
                foreach (var i in cleanup)
                {
                    if (i != null) Destroy(i);
                }
            }

            foreach (var i in spawnOnQuestComplete)
            {
                var clone = GameObject.Instantiate(i);
                clone.SetActive(true);
            }
            isFinished = true;
        }

    }
}