using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Fence Tile", menuName = "Tiles/Fence Tile")]
    public class FenceTile : TileBase
    {
        public Tile.ColliderType colliderType;
        public Sprite LT, LR, LB, TR, TB, RB, LTR, TRB, LRB, LTB, LTRB, L, T, R, B, alone;

        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    var position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    var tile = tileMap.GetTile(position);
                    if (tile == this)
                    {
                        tileMap.RefreshTile(position);
                    }
                }
        }

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.sprite = alone;
            ConfigureTile(location, tileMap, ref tileData);
            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = this.colliderType;
        }

        public void ConfigureTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            var left = tileMap.GetTile(location + Vector3Int.left) == this;
            var right = tileMap.GetTile(location + Vector3Int.right) == this;
            var top = tileMap.GetTile(location + Vector3Int.up) == this;
            var bottom = tileMap.GetTile(location + Vector3Int.down) == this;

            if (left && top && right && bottom)
            {
                tileData.sprite = LTRB;
            }
            else if (left && top && right)
            {
                tileData.sprite = LTR;
            }
            else if (left && top && bottom)
            {
                tileData.sprite = LTB;
            }
            else if (top && right && bottom)
            {
                tileData.sprite = TRB;
            }
            else if (left && right && bottom)
            {
                tileData.sprite = LRB;
            }
            else if (left && right)
            {
                tileData.sprite = LR;
            }
            else if (top && bottom)
            {
                tileData.sprite = TB;
            }
            else if (left && top)
            {
                tileData.sprite = LT;
            }
            else if (left && bottom)
            {
                tileData.sprite = LB;
            }
            else if (top && right)
            {
                tileData.sprite = TR;
            }
            else if (right && bottom)
            {
                tileData.sprite = RB;
            }
            else if (left)
            {
                tileData.sprite = L;
            }
            else if (right)
            {
                tileData.sprite = R;
            }
            else if (top)
            {
                tileData.sprite = T;
            }
            else if (bottom)
            {
                tileData.sprite = B;
            }
            else
            {
                tileData.sprite = alone;
            }
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FenceTile))]
    public class FenceTileEditor : Editor
    {
        private FenceTile tile { get { return (target as FenceTile); } }

        public void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);
        }
    }
#endif
}
