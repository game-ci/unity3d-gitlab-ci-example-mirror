using System;
using UnityEditor;
using UnityEngine;

namespace RPGM.Gameplay
{

    [CustomPropertyDrawer(typeof(Cutscene.CutsceneEvent))]
    public class CutsceneEventDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 64 + 16 + 16 + 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.y += 8;
            EditorGUI.BeginProperty(rect, label, property);
            rect.height = 16;
            var w2 = rect.width * 0.3f;
            var w1 = rect.width * 0.15f;
            var w3 = rect.width * 0.4f;
            EditorGUIUtility.labelWidth = 64;
            rect.width = position.width / 3;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("newImage"), new GUIContent("Image", "Background Image"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("transitionGradient"), new GUIContent("Transition", "Transition Gradient"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("audioClip"), new GUIContent("Audio", "Audio Clip"));
            rect.width = position.width * 0.25f;
            rect.x = position.x;
            rect.y += rect.height + 8;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("duration"), new GUIContent("Time", "Duration"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("zoom"), new GUIContent("Zoom", "Zoom"));
            rect.x += rect.width;
            rect.width = position.width * 0.5f;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("pan"), new GUIContent("Pan", "Panning"));
            rect.x = position.x;
            rect.y += rect.height + 8;
            rect.width = position.width;
            rect.height = 64;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("newText"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}