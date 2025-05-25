using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using HexKit3D;

namespace HexKit3D.Editor
{
    public class HexPlacer : EditorWindow
    {
        //placing
        HexTilemap target;
        HexTile placingPrefab;
        float heightOffset = 0.0f;
        HexPlacerMode mode;
        HexPlacerReplaceMode replaceMode;

        //controls
        float heightOffsetIncrement = 0.5f;

        //visuals
        float minTileHeight = 0.0f, maxTileHeight = 10.0f;
        Color minTileColor = Color.red, maxTileColor = Color.blue;
        HexRenderer placementPreview => EditorResources.hexPreview;

        [MenuItem("Window/3D Hex Kit/Hex Placer")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<HexPlacer>();
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Placements");
            target = EditorGUILayout.ObjectField("Hex Tilemap", target, typeof(HexTilemap), true) as HexTilemap;
            placingPrefab = EditorGUILayout.ObjectField("Placing Prefab", placingPrefab, typeof(HexTile), false) as HexTile;
            heightOffset = EditorGUILayout.FloatField("Height Offset", heightOffset);
            mode = (HexPlacerMode)EditorGUILayout.EnumPopup("Mode", mode);
            if(mode == HexPlacerMode.Placing)
            {
                replaceMode = (HexPlacerReplaceMode)EditorGUILayout.EnumPopup("Replace Mode", replaceMode);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Controls");
            heightOffsetIncrement = EditorGUILayout.FloatField("Height Offset Increment", heightOffsetIncrement);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Visuals - Tile Height");
            minTileHeight = EditorGUILayout.FloatField("Min Tile View Height", minTileHeight);
            minTileColor = EditorGUILayout.ColorField("Min Tile View Color", minTileColor);
            maxTileHeight = EditorGUILayout.FloatField("Max TIle View Height", maxTileHeight);
            maxTileColor = EditorGUILayout.ColorField("Max Tile View Color", maxTileColor);
            if (GUILayout.Button("Create Height View")) CreateHeightView();
            if (GUILayout.Button("Clear Height View")) ClearHeightView();
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += DuringSceneGui;
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            placementPreview.gameObject.SetActive(false);
            mode = HexPlacerMode.None;
            mouseHeld = false;
            if (preview != null) DestroyImmediate(preview.gameObject);
            ClearHeightView();
        }
        bool mouseHeld = false;
        Vector2 mousePos = Vector2.zero;
        Vector2 delta = Vector2.zero;
        HexTile previewPrefab = null;
        HexTile preview = null;
        private void DuringSceneGui(SceneView sceneView)
        {
            UpdateInputs();
            UpdatePlacing();
        }
        void UpdateInputs()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                mouseHeld = true;
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                mouseHeld = false;
            }
            if (Event.current.type == EventType.MouseMove)
            {
                mousePos = Event.current.mousePosition;
            }
            if (Event.current.type == EventType.MouseDrag)
            {
                delta += Event.current.delta;
                mousePos = Event.current.mousePosition + delta;
            }
            else delta = Vector2.zero;
        }
        void UpdatePlacing()
        {
            switch (mode)
            {
                case HexPlacerMode.Placing:
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) Event.current.Use();
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E)
                    {
                        Event.current.Use();
                        heightOffset += heightOffsetIncrement;
                        Repaint();
                    }
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q)
                    {
                        Event.current.Use();
                        heightOffset -= heightOffsetIncrement;
                        Repaint();
                    }
                    if (target != null && placingPrefab != null)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
                        float t = (target.transform.position.y + heightOffset - ray.origin.y) / ray.direction.y;
                        Vector3 point = ray.origin + ray.direction * t;
                        Cubic hex = Cubic.PosToCubic(target.transform.InverseTransformPoint(point), target.tileSize);

                        if (preview == null)
                        {
                            previewPrefab = placingPrefab;
                            preview = PrefabUtility.InstantiatePrefab(previewPrefab, target.transform) as HexTile;
                            preview.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.HideInInspector;
                        }
                        else if (previewPrefab != placingPrefab)
                        {
                            DestroyImmediate(preview);
                            previewPrefab = placingPrefab;
                            preview = PrefabUtility.InstantiatePrefab(previewPrefab, target.transform) as HexTile;
                        }
                        Vector3 pos = Cubic.CubicToPos(hex, target.transform.position.y + heightOffset, target.tileSize);
                        preview.transform.localPosition = pos;
                        placementPreview.gameObject.SetActive(true);
                        placementPreview.transform.position = target.transform.TransformPoint(pos) + Vector3.up * 0.05f;
                        placementPreview.outerRadius = target.tileSize;
                        placementPreview.innerRadius = target.tileSize - 0.05f;

                        if (mouseHeld)
                        {
                            HexTile found = target.placedTiles.Find(item => item.position == hex);
                            if (found == null)
                            {
                                preview.position = hex;
                                preview.owner = target;
                                preview.gameObject.hideFlags = HideFlags.None;
                                EditorUtility.SetDirty(preview);
                                PrefabUtility.RecordPrefabInstancePropertyModifications(preview);
                                target.placedTiles.Add(preview);
                                preview = null;
                            }
                            else if (replaceMode == HexPlacerReplaceMode.Replace)
                            {
                                target.placedTiles.Remove(found);
                                DestroyImmediate(found.gameObject);
                                preview.position = hex;
                                preview.owner = target;
                                preview.gameObject.hideFlags = HideFlags.None;
                                EditorUtility.SetDirty(preview);
                                PrefabUtility.RecordPrefabInstancePropertyModifications(preview);
                                target.placedTiles.Add(preview);
                                preview = null;
                            }
                        }
                    }
                    else
                    {
                        placementPreview.gameObject.SetActive(false);
                    }
                    break;
                case HexPlacerMode.Deleting:
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) Event.current.Use();
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E)
                    {
                        Event.current.Use();
                        heightOffset += heightOffsetIncrement;
                        Repaint();
                    }
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q)
                    {
                        Event.current.Use();
                        heightOffset -= heightOffsetIncrement;
                        Repaint();
                    }
                    if (target != null)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
                        float t = (target.transform.position.y + heightOffset - ray.origin.y) / ray.direction.y;
                        Vector3 point = ray.origin + ray.direction * t;
                        Cubic hex = Cubic.PosToCubic(target.transform.InverseTransformPoint(point), target.tileSize);

                        Vector3 pos = Cubic.CubicToPos(hex, target.transform.position.y + heightOffset, target.tileSize);
                        placementPreview.gameObject.SetActive(true);
                        placementPreview.transform.position = target.transform.TransformPoint(pos) + Vector3.up * 0.05f;
                        placementPreview.outerRadius = target.tileSize;
                        placementPreview.innerRadius = target.tileSize - 0.05f;

                        if (mouseHeld)
                        {
                            HexTile found = target.placedTiles.Find(item => item.position == hex);
                            if (found != null)
                            {
                                target.placedTiles.Remove(found);
                                DestroyImmediate(found.gameObject);
                            }
                        }
                    }
                    else
                    {
                        placementPreview.gameObject.SetActive(false);
                    }
                    if (preview != null) DestroyImmediate(preview.gameObject); break;
                case HexPlacerMode.None:
                    placementPreview.gameObject.SetActive(false);
                    if (preview != null) DestroyImmediate(preview.gameObject); break;
            }
        }

        readonly List<TextHexRenderer> heightViewers = new();
        readonly Dictionary<float, Material> heightMaterials = new();
        void CreateHeightView()
        {
            if(target != null)
            {
                heightViewers.RemoveAll(x => x == null);
                int i = 0;
                int index = 0;
                for(; i < target.placedTiles.Count; i++)
                {
                    if (target.placedTiles[i].height < minTileHeight || target.placedTiles[i].height > maxTileHeight) continue;
                    if(heightViewers.Count <= index)
                    {
                        TextHexRenderer tmp = Instantiate(EditorResources.hexTextPreviewPrefab);
                        tmp.gameObject.hideFlags = HideFlags.HideAndDontSave;
                        for (int k = 0; k < tmp.transform.childCount; k++) tmp.transform.GetChild(k).gameObject.hideFlags = HideFlags.HideAndDontSave;
                        heightViewers.Add(tmp);
                    }
                    heightViewers[index].gameObject.SetActive(true);
                    heightViewers[index].transform.position = target.placedTiles[i].transform.position + Vector3.up * 0.025f;
                    heightViewers[index].outerRadius = target.tileSize;
                    float height = target.placedTiles[i].height;
                    heightViewers[index].textMesh.text = height.ToString();
                    if (!heightMaterials.ContainsKey(height))
                    {
                        heightMaterials[height] = new Material(EditorResources.hexTextPreviewPrefab.meshRenderer.sharedMaterial);
                    }
                    float t = Mathf.Clamp01((height - minTileHeight) / (maxTileHeight - minTileHeight));
                    heightMaterials[height].color = Color.Lerp(minTileColor, maxTileColor, t);
                    heightViewers[index].meshRenderer.material = heightMaterials[height];
                    index++;
                }
                for(; index < heightViewers.Count; index++) heightViewers[index].gameObject.SetActive(false);
            }
        }
        void ClearHeightView()
        {
            heightViewers.RemoveAll(x => x == null);
            for (int i = 0; i < heightViewers.Count; i++) heightViewers[i].gameObject.SetActive(false);
        }
    }
    [System.Serializable]
    public enum HexPlacerMode
    {
        None = 0,
        Placing = 1,
        Deleting = 2
    }
    [System.Serializable]
    public enum HexPlacerReplaceMode
    {
        Ignore = 0,
        Replace = 1
    }
}
/*public class HexPlacer : ScriptableWizard
{
    [SerializeField] HexTilemap target;
    [SerializeField] HexTile placingPrefab;
    [SerializeField] float placingHeight;

    bool placing = false;
    [MenuItem("Tools/Hex Placer")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        ScriptableWizard.DisplayWizard<HexPlacer>(
            "Hex Placer", "Create", "Start Placing");
    }
    private void OnWizardCreate()
    {
        
    }
    private void OnWizardOtherButton()
    {
        placing = !placing;
        otherButtonName = placing ? "Stop Placing" : "Start Placing";
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGui;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGui;
    }
    void DuringSceneGui(SceneView sceneView)
    {
        if (placing)
        {
            Ray ray = sceneView.camera.ScreenPointToRay(Event.current.mousePosition);
            float t = (placingHeight - ray.origin.y) / ray.direction.y;
            Vector3 point = ray.origin + ray.direction * t;
            Debug.Log(point);
            Debug.DrawLine(new Vector3(point.x, Mathf.Infinity, point.z), new Vector3(point.x, Mathf.NegativeInfinity, point.z));
        }
    }
}*/
