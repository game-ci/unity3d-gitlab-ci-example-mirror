using System.Collections.Generic;
using System.Linq;
using RPGM.Gameplay;
using UnityEditor;
using UnityEngine;

namespace RPGM.EditorExtensions
{
    public class InventoryItemSearchDialog : ScriptableWizard
    {

        public StoryItem si;

        InventoryItem[] items;
        List<InventoryItem> results = new List<InventoryItem>();
        GUIStyle buttonStyle;
        System.Action<InventoryItem> onSelect = null;

        string query = "";

        void OnEnable()
        {
            buttonStyle = EditorStyles.miniButton;
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            items = GameObject.FindObjectsOfType<InventoryItem>();
            items.OrderBy(i => i.name);
            results.AddRange(items);
        }

        public static void Show(StoryItem target, System.Action<InventoryItem> onSelect)
        {
            var w = ScriptableWizard.DisplayWizard<InventoryItemSearchDialog>("Inventory Item Search", "Close");
            w.si = target;
            w.onSelect = onSelect;
        }

        void OnWizardCreate()
        {

        }

        void Update()
        {

        }

        protected override bool DrawWizardGUI()
        {
            var lastQuery = query;
            query = GUILayout.TextField(query);
            if (lastQuery != query)
            {
                Search();
            }
            foreach (var i in results)
            {
                GUILayout.Space(5);
                if (GUILayout.Button(i.name, buttonStyle))
                {
                    if (onSelect != null)
                        onSelect(i);
                    Close();
                }
            }
            return false;
        }

        void Search()
        {
            results.Clear();
            results.AddRange(from i in items orderby i.name where Match(query, i) select i);
        }

        bool Match(string query, InventoryItem i)
        {
            var id = i.name.ToLower();
            var q = query.ToLower();
            if (id != string.Empty)
            {
                if (id.StartsWith(q)) return true;
                if (id.EndsWith(q)) return true;
                if (id.Contains(q)) return true;
            }
            return false;
        }
    }
}