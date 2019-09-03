using System;
using System.Collections.Generic;
using System.Linq;
using RPGM.Gameplay;
using UnityEditor;
using UnityEngine;

namespace RPGM.EditorExtensions
{
    public class StoryItemSearchDialog : ScriptableWizard
    {

        public StoryItem si;

        StoryItem[] items;
        List<StoryItem> results = new List<StoryItem>();
        GUIStyle buttonStyle;
        System.Action<StoryItem> onSelect = null;

        string query = "";

        void OnEnable()
        {
            buttonStyle = EditorStyles.miniButton;
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            items = GameObject.FindObjectsOfType<StoryItem>();
            items.OrderBy(i => i.text);
            results.AddRange(items);
        }

        public static void Show(StoryItem target, System.Action<StoryItem> onSelect)
        {
            var w = ScriptableWizard.DisplayWizard<StoryItemSearchDialog>("Story Item Search", "Close");
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
                if (GUILayout.Button(i.text, buttonStyle))
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
            results.AddRange(from i in items orderby i.text where Match(query, i) select i);
        }

        bool Match(string query, StoryItem i)
        {
            var id = i.ID.ToLower();
            var text = i.text.ToLower();
            var q = query.ToLower();
            if (id != string.Empty)
            {
                if (id.StartsWith(q)) return true;
                if (id.EndsWith(q)) return true;
                if (id.Contains(q)) return true;
            }
            if (text != string.Empty)
            {
                if (text.StartsWith(q)) return true;
                if (text.EndsWith(q)) return true;
                if (text.Contains(q)) return true;
            }
            return false;
        }
    }
}