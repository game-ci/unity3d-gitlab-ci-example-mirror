using System;
using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RPGM.EditorExtensions
{
    [CustomEditor(typeof(StoryItem))]
    public class StoryItemEditor : Editor
    {
        readonly int textLength = 32;
        readonly Vector2 size = new Vector2(210, 480);

        GUIStyle buttonStyle;

        int panel = 0;

        void OnEnable()
        {
            var sis = GameObject.FindObjectsOfType<StoryItem>();
            foreach (var i in sis)
                i.ConnectRelations();
        }

        bool ShouldDrawGUI()
        {
            var si = target as StoryItem;
            return HandleUtility.GetHandleSize(si.transform.position) <= 0.5f;
        }

        void OnSceneGUI()
        {
            if (!ShouldDrawGUI()) return;
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(EditorStyles.miniButton);
                buttonStyle.alignment = TextAnchor.MiddleLeft;
            }
            var si = target as StoryItem;
            // var pos = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(si.transform.position);
            // var pos = HandleUtility.WorldToGUIPoint(si.transform.position);
            var pos = HandleUtility.WorldToGUIPointWithDepth(si.transform.position);
            // pos.y = Screen.height - pos.y;


            var rect = new Rect(pos, size);

            Handles.BeginGUI();
            EditorGUI.BeginChangeCheck();


            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                Selection.activeGameObject = null;

            GUILayout.BeginArea(rect);
            panel = GUILayout.Toolbar(panel, new[] { "Config", "Story", "Inventory" });
            switch (panel)
            {
                case 0:
                    DrawConfig(si);
                    break;
                case 1:
                    DrawStoryItems(si);
                    break;
                case 2:
                    DrawInventoryItems(si);
                    break;

            }

            GUILayout.EndArea();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                si.gameObject.name = $"Story: {Escape(si.text)}";
            }

            Handles.EndGUI();
        }

        void DrawConfig(StoryItem si)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Text");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("text"), GUIContent.none);
            GUILayout.Label("Audio (Optional)");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClip"), GUIContent.none);
            GUILayout.Label("Disable When Discovered?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("disableWhenDiscovered"), GUIContent.none);
            GUILayout.Label("ID (Optional)");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"), GUIContent.none);
            GUILayout.EndVertical();
        }

        void DrawInventoryItems(StoryItem si)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (GUILayout.Button("Add Required Inventory Item"))
            {
                InventoryItemSearchDialog.Show(si, (i) =>
                {
                    var p = serializedObject.FindProperty("_requiredInventoryItems");
                    p.InsertArrayElementAtIndex(0);
                    p.GetArrayElementAtIndex(0).objectReferenceValue = i;
                    serializedObject.ApplyModifiedProperties();
                });
            }
            InventoryItem remove = null;

            foreach (var i in si.requiredInventoryItems)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                if (GUILayout.Button(new GUIContent("", "Remove"), EditorStyles.radioButton, GUILayout.Width(16)))
                {
                    remove = i;
                }
                GUI.color = Color.white;
                if (GUILayout.Button(Escape(i.name), buttonStyle))
                {
                    Selection.activeGameObject = i.gameObject;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                GUILayout.EndHorizontal();
            }
            if (remove != null)
            {
                var p = serializedObject.FindProperty("_requiredInventoryItems");
                //I can't believe this API is real.
                var removeIndex = -1;
                for (var i = 0; i < p.arraySize; i++)
                {
                    if (p.GetArrayElementAtIndex(i).objectReferenceValue == remove)
                    {
                        removeIndex = i;
                        break;
                    }
                }
                p.DeleteArrayElementAtIndex(removeIndex);
                p.DeleteArrayElementAtIndex(removeIndex);
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.EndVertical();
        }

        void DrawStoryItems(StoryItem si)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (si.requiredStoryItems.Count > 0)
            {
                EditorGUILayout.LabelField("Required Story Items");
                DrawRequiredRelationList(si);
            }
            if (GUILayout.Button("Add Required Story Item"))
            {
                StoryItemSearchDialog.Show(si, (i) =>
                {
                    i.dependentStoryItems.Add(si);
                    si.requiredStoryItems.Add(i);
                    var p = serializedObject.FindProperty("_requiredStoryItems");
                    p.InsertArrayElementAtIndex(0);
                    p.GetArrayElementAtIndex(0).objectReferenceValue = i;
                    serializedObject.ApplyModifiedProperties();
                });
            }
            if (si.dependentStoryItems.Count > 0)
            {
                EditorGUILayout.LabelField("Dependent Story Items");
                DrawDependentRelationList(si);
            }
            GUILayout.EndVertical();
        }

        string Escape(string text)
        {
            text = text.Replace("\n", " ");
            if (text.Length > textLength)
                text = text.Substring(0, textLength - 3) + "...";
            return text;
        }

        void DrawDependentRelationList(StoryItem si)
        {
            var remove = DrawRelationList(si.dependentStoryItems);
            if (remove != null)
            {
                var so = new SerializedObject(remove);
                var p = so.FindProperty("_requiredStoryItems");
                var removeIndex = -1;
                for (var i = 0; i < p.arraySize; i++)
                {
                    if (p.GetArrayElementAtIndex(i).objectReferenceValue == si)
                    {
                        removeIndex = i;
                        break;
                    }
                }
                //I can't believe this API is real.
                p.DeleteArrayElementAtIndex(removeIndex);
                p.DeleteArrayElementAtIndex(removeIndex);
                si.dependentStoryItems.Remove(remove);
                so.ApplyModifiedProperties();
            }
        }

        void DrawRequiredRelationList(StoryItem si)
        {
            var remove = DrawRelationList(si.requiredStoryItems);
            if (remove != null)
            {
                var p = serializedObject.FindProperty("_requiredStoryItems");
                //I can't believe this API is real.
                var removeIndex = -1;
                for (var i = 0; i < p.arraySize; i++)
                {
                    if (p.GetArrayElementAtIndex(i).objectReferenceValue == remove)
                    {
                        removeIndex = i;
                        break;
                    }
                }
                p.DeleteArrayElementAtIndex(removeIndex);
                p.DeleteArrayElementAtIndex(removeIndex);
                remove.dependentStoryItems.Remove(si);
                serializedObject.ApplyModifiedProperties();
            }
        }

        StoryItem DrawRelationList(HashSet<StoryItem> storyItems)
        {
            StoryItem remove = null;

            foreach (var i in storyItems)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                if (GUILayout.Button(new GUIContent("", "Remove"), EditorStyles.radioButton, GUILayout.Width(16)))
                {
                    remove = i;
                }
                GUI.color = Color.white;
                if (GUILayout.Button(Escape(i.text), buttonStyle))
                {
                    Selection.activeGameObject = i.gameObject;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                GUILayout.EndHorizontal();
            }

            return remove;
        }
    }
}