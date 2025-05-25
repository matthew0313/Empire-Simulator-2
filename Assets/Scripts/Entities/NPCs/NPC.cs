using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

public abstract class NPC : Entity
{
    [Header("Assigns")]
    [SerializeField] Island m_assignedIsland;
    public Island assignedIsland
    {
        get => m_assignedIsland;
        set
        {
            if(m_assignedIsland != value)
            {
                var prev = m_assignedIsland;
                m_assignedIsland = value;
                onAssignedIslandChange?.Invoke(prev, m_assignedIsland);
            }
        }
    }
    public Action<Island, Island> onAssignedIslandChange;

    [Header("Navigation")]
    public float moveSpeedMultiplier = 1.0f;


    public IResidental home { get; private set; }
    public Cubic position => assignedIsland.tilemap.PosToCubic(transform.position);
    private void Update()
    {
        RotationUpdate();
        GroundCheckUpdate();
    }
    //rotation
    float rotateRate => GameManager.Instance.npcRotateRate;
    protected float targetRotation = 0.0f;
    void RotationUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateRate * Time.deltaTime);
    }

    //height
    float heightRate => GameManager.Instance.npcHeightRate;
    void GroundCheckUpdate()
    {
        if(assignedIsland.tilemap.TryGetTile(position, out MapTile tile))
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, tile.height, heightRate * Time.deltaTime), transform.position.z);
        }
    }

    //navigation
    public bool isNavigating { get; private set; } = false;
    float moveSpeed => GameManager.Instance.baseNPCMoveSpeed * moveSpeedMultiplier;
    float stopDistance => GameManager.Instance.npcNavigationStopDistance;
    (CoroutineHandle, Action) navigating;
    public void Navigate(HexTilemapPath<MapTile> path, Action onFinish)
    {
        if (isNavigating) StopNavigating();
        if(path == null)
        {
            onFinish?.Invoke(); return;
        }
        isNavigating = true;
        navigating = (Timing.RunCoroutine(Navigating(path, onFinish)), onFinish);
    }
    public void StopNavigating()
    {
        if (!isNavigating) return;
        Timing.KillCoroutines(navigating.Item1);
        isNavigating = false;
        navigating.Item2.Invoke();
    }
    Vector3 currentPos => new Vector3(transform.position.x, 0.0f, transform.position.z);
    IEnumerator<float> Navigating(HexTilemapPath<MapTile> path, Action onFinish)
    {
        foreach(var next in path.route)
        {
            Vector3 targetPos = new Vector3(next.transform.position.x, 0.0f, next.transform.position.z);
            while (Vector3.Distance(currentPos, targetPos) > stopDistance)
            {
                if (!next.isWalkable)
                {
                    isNavigating = false;
                    onFinish?.Invoke();
                    yield break;
                }
                targetRotation = Mathf.Atan2(targetPos.z - currentPos.z, targetPos.x - currentPos.x) * Mathf.Rad2Deg;
                transform.Translate(Vector3.ClampMagnitude((targetPos - currentPos).normalized * moveSpeed * Time.deltaTime, Vector3.Distance(targetPos, currentPos)));
                yield return Timing.WaitForOneFrame;
            }
        }
        isNavigating = false;
        onFinish?.Invoke();
    }
    protected abstract class NPC_FSMVals : FSMVals
    {

    }
    protected abstract class NPC_TopLayer<T> : TopLayer<T> where T : NPC
    {
        protected NPC_TopLayer(T origin, NPC_FSMVals values) : base(origin, values)
        {

        }
        protected abstract class NPC_Homeless<T> : State<T> where T : NPC
        {
            protected NPC_Homeless(T origin, Layer<T> parent) : base(origin, parent)
            {

            }
        }
    }
}