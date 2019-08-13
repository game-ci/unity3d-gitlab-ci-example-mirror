using System;
using System.Collections;
using System.Collections.Generic;
using RPGM.Core;
using RPGM.Gameplay;
using RPGM.UI;
using UnityEngine;

namespace RPGM.Gameplay
{
    /// <summary>
    /// This class provides all the data you need to control and change gameplay.
    /// </summary>
    [Serializable]
    public class GameModel
    {
        public CharacterController2D player;
        public DialogController dialog;
        public InputController input;
        public InventoryController inventoryController;
        public MusicController musicController;

        Dictionary<GameObject, HashSet<string>> conversations = new Dictionary<GameObject, HashSet<string>>();

        Dictionary<string, int> inventory = new Dictionary<string, int>();
        Dictionary<string, Sprite> inventorySprites = new Dictionary<string, Sprite>();

        HashSet<string> storyItems = new HashSet<string>();

        public IEnumerable<string> InventoryItems => inventory.Keys;

        public Sprite GetInventorySprite(string name)
        {
            Sprite s;
            inventorySprites.TryGetValue(name, out s);
            return s;
        }

        public int GetInventoryCount(string name)
        {
            int c;
            inventory.TryGetValue(name, out c);
            return c;
        }

        public void AddInventoryItem(InventoryItem item)
        {
            int c = 0;
            inventory.TryGetValue(item.name, out c);
            c += item.count;
            inventorySprites[item.name] = item.sprite;
            inventory[item.name] = c;
            inventoryController.Refresh();
        }

        public bool HasInventoryItem(string name, int count = 1)
        {
            int c = 0;
            inventory.TryGetValue(name, out c);
            return c >= count;
        }

        public bool RemoveInventoryItem(InventoryItem item, int count)
        {
            int c = 0;
            inventory.TryGetValue(item.name, out c);
            c -= count;
            if (c < 0) return false;
            inventory[item.name] = c;
            inventoryController.Refresh();
            return true;
        }

        public void RegisterStoryItem(string ID)
        {
            storyItems.Add(ID);
        }

        public bool HasSeenStoryItem(string ID)
        {
            return storyItems.Contains(ID);
        }

        public void RegisterConversation(GameObject owner, string id)
        {
            if (!conversations.TryGetValue(owner, out HashSet<string> ids))
                conversations[owner] = ids = new HashSet<string>();
            ids.Add(id);
        }

        public bool HasHadConversationWith(GameObject owner, string id)
        {
            if (!conversations.TryGetValue(owner, out HashSet<string> ids))
                return false;
            return ids.Contains(id);
        }

        public bool HasMet(GameObject owner)
        {
            return conversations.ContainsKey(owner);
        }
    }
}