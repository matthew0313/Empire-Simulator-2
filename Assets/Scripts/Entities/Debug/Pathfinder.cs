using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class Pathfinder : Entity
{
    [SerializeField] Island assignedIsland;
    [SerializeField] Cubic target;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateRate = 4.0f, heightRate = 4.0f;
    [SerializeField] float stopDistance = 0.01f;
    public bool isNavigating { get; private set; } = false;
    private void Update()
    {
        RotationUpdate();
        GroundCheckUpdate();
    }
    private void OnDrawGizmos()
    {
        if (navigatingPath != null)
        {
            for(int i = 1; i < navigatingPath.route.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(navigatingPath.route[i - 1].transform.position, navigatingPath.route[i].transform.position);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 pos = assignedIsland.tilemap.CubicToPos(position);
        Gizmos.DrawLine(pos + Vector3.up * 10000.0f, pos + Vector3.down * 10000.0f);

        Gizmos.color = Color.red;
        pos = assignedIsland.tilemap.CubicToPos(target);
        Gizmos.DrawLine(pos + Vector3.up * 10000.0f, pos + Vector3.down * 10000.0f);
    }
    public Cubic position => assignedIsland.tilemap.PosToCubic(transform.position);
    protected float targetRotation = 0.0f;
    void RotationUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateRate * Time.deltaTime);
    }
    void GroundCheckUpdate()
    {
        if (assignedIsland.tilemap.TryGetTile(position, out MapTile tile))
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, tile.height, heightRate * Time.deltaTime), transform.position.z);
        }
    }
    CoroutineHandle navigating;
    HexTilemapPath<MapTile> navigatingPath;
    public void Navigate()
    {
        if (isNavigating) return;
        navigatingPath = assignedIsland.tilemap.FindPath<MapTile>(position, target);
        if(navigatingPath != null)
        {
            isNavigating = true;
            navigating = Timing.RunCoroutine(Navigating(navigatingPath, null));
        }
    }
    public void StopNavigating()
    {
        if(!isNavigating) return;
        Timing.KillCoroutines(navigating);
        isNavigating = false;
    }
    Vector3 currentPos => new Vector3(transform.position.x, 0.0f, transform.position.z);
    IEnumerator<float> Navigating(HexTilemapPath<MapTile> path, Action onFinish)
    {
        foreach (var next in path.route)
        {
            Vector3 targetPos = new Vector3(next.transform.position.x, 0.0f, next.transform.position.z);
            while (Vector3.Distance(currentPos, targetPos) > stopDistance)
            {
                if (!next.isWalkable)
                {
                    onFinish?.Invoke();
                    isNavigating = false;
                    yield break;
                }
                targetRotation = Mathf.Atan2(targetPos.x - currentPos.x, targetPos.z - currentPos.z) * Mathf.Rad2Deg;
                transform.Translate(Vector3.ClampMagnitude((targetPos - currentPos).normalized * moveSpeed * Time.deltaTime, Vector3.Distance(targetPos, currentPos)), Space.World);
                yield return Timing.WaitForOneFrame;
            }
        }
        onFinish?.Invoke();
        isNavigating = false;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Pathfinder))]
public class Pathfinder_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Pathfind")) (target as Pathfinder).Navigate();
    }
}
#endif