using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using HexKit3D;

/*namespace HexKit3D.Editor
{
    public class HexPathfinder : EditorWindow
    {
        HexTile startTile, endTile;
        [MenuItem("Window/3D Hex Kit/Hex Pathfinder")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<HexPathfinder>();
        }
        private void OnGUI()
        {
            startTile = EditorGUILayout.ObjectField("Start Tile", startTile, typeof(HexTile), true) as HexTile;
            endTile = EditorGUILayout.ObjectField("End Tile", endTile, typeof(HexTile), true) as HexTile;
            if (startTile != null && endTile != null)
            {
                if(startTile.owner == endTile.owner)
                {
                    if (GUILayout.Button("Search Path")) SearchPath();
                }
                else
                {
                    GUILayout.Label("Start tile's owner does not match end tile's owner");
                }
            }
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += DuringSceneGui;
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
        }
        HexTilemapPath path;
        private void Update()
        {
            if (path != null)
            {
                Debug.Log("ADAWDSAW");
                for(int i = 0; i < path.route.Count - 1; i++)
                {
                    Debug.DrawLine(path.route[i].transform.position, path.route[i + 1].transform.position, Color.cyan);
                }
            }
        }
        void DuringSceneGui(SceneView sceneView)
        {

        }
        void SearchPath()
        {
            path = startTile.owner.FindPath(startTile.position, endTile.position);
        }
    }
}*/