using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.SceneManagement;
using RPGM.Gameplay;

namespace RPGM.Gameplay
{

    public class ConversationPieceDialog : ScriptableWizard
    {

        public ConversationPiece conversationPiece, originalConversationPiece;

        public ReorderableList options;
        ConversationScript conversationScript;
        bool isUpdate = false;
        string[] targets;

        internal static void New(ConversationScript conversationScript)
        {
            var w = ScriptableWizard.DisplayWizard<ConversationPieceDialog>("New Conversation Piece", "Create");
            w.conversationScript = conversationScript;
            w.conversationPiece = new ConversationPiece() { id = "", text = "", options = new List<ConversationOption>() };
            w.isUpdate = false;
        }

        internal static void Edit(ConversationScript conversationScript, ConversationPiece conversationPiece)
        {
            var w = ScriptableWizard.DisplayWizard<ConversationPieceDialog>("Edit Conversation Piece", "Update");
            w.targets = (from i in conversationScript.items select i.id).ToArray();
            w.originalConversationPiece = conversationPiece;
            w.conversationPiece = conversationPiece;
            w.conversationScript = conversationScript;
            w.isUpdate = true;
        }

        void BuildOptionList()
        {
            options = new ReorderableList(conversationPiece.options, typeof(ConversationOption), true, true, true, true);
            options.drawElementCallback = OnDrawOption;
            options.drawHeaderCallback = OnDrawOptionHeader;
        }

        void OnDrawOptionHeader(Rect rect)
        {
            GUI.Label(rect, "Branches");
        }

        void OnDrawOption(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = conversationPiece.options[index];
            var i = System.Array.IndexOf(targets, item.targetId);
            if (i < 0) i = 0;
            var r = rect;
            r.height = 16;
            r.width = rect.width * 0.2f;
            i = EditorGUI.Popup(r, i, targets);
            r.x += r.width;
            item.targetId = targets[i];
            item.image = (Sprite)EditorGUI.ObjectField(r, item.image, typeof(Sprite), false);
            r.x += r.width;
            r.width = rect.width * 0.6f;
            item.text = EditorGUI.TextField(r, item.text);
            conversationPiece.options[index] = item;
        }

        private void OnAdd(ReorderableList list)
        {
            list.list.Add(new ConversationOption() { targetId = "", text = "", image = null, enabled = true });
        }

        void OnWizardCreate()
        {
            if (isUpdate)
            {
                Undo.RecordObject(conversationScript, "Update Item");
                conversationScript.Set(originalConversationPiece, conversationPiece);
                EditorUtility.SetDirty(conversationScript);
            }
            else
            {
                Undo.RecordObject(conversationScript, "Add Item");
                conversationScript.Add(conversationPiece);
                EditorUtility.SetDirty(conversationScript);
            }
        }

        void Update()
        {

        }

        protected override bool DrawWizardGUI()
        {
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                return true;
            }
            isValid = true;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel("ID");
            conversationPiece.id = EditorGUILayout.TextField(conversationPiece.id).Trim().ToUpper();
            if (conversationPiece.id.Length == 0)
            {
                EditorGUILayout.HelpBox("The ID field cannot be empty.", MessageType.Error);
                isValid = false;
            }
            else if (isUpdate && conversationPiece.id != originalConversationPiece.id)
            {
                if (conversationScript.ContainsKey(conversationPiece.id))
                {
                    EditorGUILayout.HelpBox("This ID already exists in this conversation and cannot be saved.", MessageType.Error);
                    isValid = false;
                }
                else
                {
                    EditorGUILayout.HelpBox("ID has changed and will be updated in related records", MessageType.Warning);
                }
            }

            EditorGUILayout.PrefixLabel("Image (Optional)");
            conversationPiece.image = (Sprite)EditorGUILayout.ObjectField(conversationPiece.image, typeof(Sprite), false);
            EditorGUILayout.PrefixLabel("Text");
            conversationPiece.text = EditorGUILayout.TextArea(conversationPiece.text);
            conversationPiece.audio = (AudioClip)EditorGUILayout.ObjectField(conversationPiece.audio, typeof(AudioClip), false);
            EditorGUILayout.PrefixLabel("Quest (Optional)");
            conversationPiece.quest = (Quest)EditorGUILayout.ObjectField(conversationPiece.quest, typeof(Quest), true);

            if (conversationScript.items.Count > 0)
            {
                if (options == null) BuildOptionList();
                options.DoLayoutList();
            }

            return EditorGUI.EndChangeCheck();
        }
    }
}