using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEngine;

namespace RPGM.Gameplay
{
    /// <summary>
    /// A single conversation container.
    /// </summary>
    public class ConversationScript : MonoBehaviour, ISerializationCallbackReceiver
    {

        [HideInInspector] [SerializeField] public List<ConversationPiece> items = new List<ConversationPiece>();
        Dictionary<string, ConversationPiece> index = new Dictionary<string, ConversationPiece>();

        public bool ContainsKey(string id)
        {
            return index.ContainsKey(id);
        }

        public ConversationPiece Get(string id)
        {
            return index[id];
        }

        public void Add(ConversationPiece conversationPiece)
        {
            items.Add(conversationPiece);
        }

        public void Set(ConversationPiece originalConversationPiece, ConversationPiece newConversationPiece)
        {
            if (originalConversationPiece.id != newConversationPiece.id)
            {
                foreach (var i in items)
                {
                    var options = i.options;
                    for (var j = 0; j < options.Count; j++)
                    {
                        if (options[j].targetId == originalConversationPiece.id)
                        {
                            var c = options[j];
                            c.targetId = newConversationPiece.id;
                            options[j] = c;
                        }
                    }
                }
            }
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].id == originalConversationPiece.id)
                {
                    items[i] = newConversationPiece;
                    break;
                }
            }
        }

        public void Delete(string id)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    items.RemoveAt(i);
                    break;
                }
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            index.Clear();
            foreach (var i in items)
                index[i.id] = i;
        }
    }
}