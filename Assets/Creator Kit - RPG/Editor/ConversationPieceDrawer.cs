using UnityEditor;
using UnityEngine;

namespace RPGM.Gameplay
{

    // [CustomPropertyDrawer(typeof(ConversationPiece))]
    public class ConversationPieceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 190;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = 16;
            EditorGUI.BeginProperty(rect, label, property);
            rect.width = position.width * 0.2f;
            EditorGUIUtility.labelWidth = 32;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("id"), new GUIContent("ID"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("image"), GUIContent.none);
            rect.x += rect.width - 16;
            rect.x = position.width - rect.xMax;
            rect.width = position.width - rect.x;
            rect.height = 64;
            rect = EditorGUI.PrefixLabel(rect, new GUIContent("Text"));
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("text"), GUIContent.none);
            rect.x += rect.height;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("options"), true);
            EditorGUI.EndProperty();
        }
    }

    // [CustomPropertyDrawer(typeof(ConversationOption))]
    public class ConversationOptionDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = 16;
            rect.width = position.width * 0.25f;
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetId"), GUIContent.none);
            rect.x += rect.width;
            rect.width = 72;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("image"), GUIContent.none);
            rect.x += rect.width;
            rect.width = position.width * 0.25f;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("text"), GUIContent.none);
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("enabled"), GUIContent.none);
            EditorGUI.EndProperty();
        }

    }
}