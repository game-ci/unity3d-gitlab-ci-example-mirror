using System;
using System.Collections;
using System.Collections.Generic;
using RPGM.EditorExtensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPGM.Gameplay
{
    [CustomEditor(typeof(ConversationScript), true)]
    public class ConversationScriptEditor : Editor
    {
        ReorderableList list;
        ConversationScript script;

        void OnEnable()
        {
            script = target as ConversationScript;
            // list = new ReorderableList(serializedObject, serializedObject.FindProperty("items"), true, true, true, true);
            list = new ReorderableList(script.items, typeof(ConversationPiece), true, true, true, true);
            list.drawElementCallback = OnDrawElement;
            list.onAddCallback += OnAdd;
            list.onRemoveCallback += OnRemove;
            list.drawHeaderCallback += OnDrawHeader;
            list.onSelectCallback += OnSelect;
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnSelect(ReorderableList list)
        {

        }

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Conversation Script Items");
        }

        void OnUndoRedo()
        {
            if (serializedObject != null)
                serializedObject.Update();
        }

        void OnRemove(ReorderableList list)
        {
            var item = script.items[list.index];
            Undo.RecordObject(target, "Remove Item");
            script.Delete(item.id);
        }

        void OnAdd(ReorderableList list)
        {
            ConversationPieceDialog.New(script);
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = (ConversationPiece)list.list[index];
            var r = rect;
            r.width = rect.width * 0.2f;
            GUI.Label(r, item.id, EditorStyles.boldLabel);
            r.x += r.width;
            r.width = rect.width * 0.7f;
            GUI.Label(r, item.text + (item.quest != null ? $" ({item.quest.title})" : ""));
            r.x += r.width;
            r.width = rect.width * 0.1f;
            r.y -= 1;
            r.height -= 2;
            if (list.index == index)
            {
                if (GUI.Button(r, "Edit", EditorStyles.miniButton))
                {
                    ConversationPieceDialog.Edit(script, item);
                }
            }
        }

        public override void OnInspectorGUI()
        {

            var questProperty = serializedObject.FindProperty("quest");
            if (questProperty != null)
                EditorGUILayout.PropertyField(questProperty, true);
            serializedObject.ApplyModifiedProperties();
            list.DoLayoutList();
        }
    }
}