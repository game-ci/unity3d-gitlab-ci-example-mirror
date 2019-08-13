using System.Collections;
using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEditor;
using UnityEngine;

namespace RPGM.EditorExtensions
{

    public class MechanicsGizmos
    {
        static bool IzZoomed(Transform t)
        {
            return ((Camera.current.transform.position - t.position).magnitude <= 20);
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawInventoryItemGizmos(InventoryItem ii, GizmoType gizmoType)
        {
            if (IzZoomed(ii.transform))
            {

            }
            else
            {
                Gizmos.DrawIcon(ii.transform.position, "inventory");
            }
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawStoryItemGizmos(StoryItem si, GizmoType gizmoType)
        {
            if (IzZoomed(si.transform))
            {
                if (si.requiredStoryItems != null)
                    foreach (var i in si.requiredStoryItems)
                    {
                        Handles.color = new Color(1, 0.99f, 0);
                        var delta = (i.transform.position - si.transform.position).normalized;
                        var left = Vector3.Cross(Vector3.forward, delta);
                        var start = i.transform.position - delta * 0.25f;
                        var end = si.transform.position + delta * 0.25f;
                        Handles.DrawDottedLine(end, start, 3);
                        Handles.DrawDottedLine(end, end + (left + delta) * 0.1f, 2);
                        Handles.DrawDottedLine(end, end - (left - delta) * 0.1f, 2);
                    }
                if (si.requiredInventoryItems != null)
                    foreach (var i in si.requiredInventoryItems)
                    {
                        Handles.color = new Color(1, 0.99f, 0);
                        var delta = (i.transform.position - si.transform.position).normalized;
                        var left = Vector3.Cross(Vector3.forward, delta);
                        var start = i.transform.position - delta * 0.25f;
                        var end = si.transform.position + delta * 0.25f;
                        Handles.DrawDottedLine(end, start, 3);
                        Handles.DrawDottedLine(end, end + (left + delta) * 0.1f, 2);
                        Handles.DrawDottedLine(end, end - (left - delta) * 0.1f, 2);
                    }

                if (si.text != null && si.text != string.Empty)
                {
                    Handles.Label(si.transform.position, si.text, EditorStyles.helpBox);
                }
            }
            else
            {
                Gizmos.DrawIcon(si.transform.position, "story");
            }
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawQuestGizmos(Quest q, GizmoType gizmoType)
        {
            if (IzZoomed(q.transform))
            {
                var rect = new Rect(Camera.current.WorldToScreenPoint(q.transform.position), new Vector2(128, 16));
                rect.y = Screen.height - rect.y;
                Handles.BeginGUI();
                GUI.Label(rect, $"Quest: {q.title}", EditorStyles.helpBox);
                Handles.EndGUI();
            }
            else
            {
                Gizmos.DrawIcon(q.transform.position, "quest");
            }
        }
    }
}