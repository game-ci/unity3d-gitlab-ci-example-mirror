using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;

namespace RPGM.EditorExtensions
{
    [InitializeOnLoad]
    public static class SceneContext
    {
        static readonly string[] specialFolders = new[] {
            "Assets/Creator Kit - RPG/Gameplay Prefabs"
        };

        static System.Diagnostics.Stopwatch clickClock;
        static Vector3 mousePosition;

        static SceneContext()
        {
            clickClock = new System.Diagnostics.Stopwatch();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneview)
        {
            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown)
                    clickClock.Start();
                if (Event.current.type == EventType.MouseUp)
                {
                    clickClock.Stop();
                    var period = clickClock.ElapsedMilliseconds;
                    clickClock.Reset();
                    if (period < 300)
                    {

                        var mousePos = Event.current.mousePosition;
                        float ppp = EditorGUIUtility.pixelsPerPoint;
                        mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y * ppp;
                        mousePos.x *= ppp;

                        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);

                        mousePosition = ray.origin;
                        mousePosition.z = 0;
                        OnContextClick();
                        Event.current.Use();
                    }
                }
            }
        }


        static void OnContextClick()
        {
            var menu = new GenericMenu();
            foreach (var i in AssetDatabase.FindAssets("t:GameObject", specialFolders))
            {
                var path = AssetDatabase.GUIDToAssetPath(i);
                var g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var menuPath = System.IO.Directory.GetParent(path);
                menu.AddItem(new GUIContent($"{menuPath.Name}/Add {g.name} Here"), false, CreateItem, g);
            }
            menu.ShowAsContext();
        }

        private static void CreateItem(object obj)
        {
            var g = obj as GameObject;
            var i = PrefabUtility.InstantiatePrefab(g) as GameObject;
            Undo.RegisterCreatedObjectUndo(i, $"Add {i.name}");
            i.transform.position = mousePosition;
            var parent = GameObject.Find($"/{i.name} Collection");
            if (parent != null)
                i.transform.parent = parent.transform;
            else if (Selection.activeTransform != null)
            {
                i.transform.parent = Selection.activeTransform;
            }
            Selection.activeGameObject = i;
        }
    }
}