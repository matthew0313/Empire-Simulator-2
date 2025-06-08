using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEditor.ShaderGraph.Internal;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;
    [SerializeField] Island m_currentIsland;
    public Island currentIsland => m_currentIsland;

    [Header("Saving")]
    [SerializeField] PrefabDatabase m_placedMapElementDB;
    public PrefabDatabase placedMapElementDB => m_placedMapElementDB;

    [Header("NPC")]
    [SerializeField] float m_baseNPCMoveSpeed = 5.0f;
    [SerializeField] float m_npcNavigationStopDistance = 0.01f;
    [SerializeField] float m_npcRotateRate = 4.0f;
    [SerializeField] float m_npcHeightRate = 4.0f;
    public float baseNPCMoveSpeed => m_baseNPCMoveSpeed;
    public float npcNavigationStopDistance => m_npcNavigationStopDistance;
    public float npcRotateRate => m_npcRotateRate;
    public float npcHeightRate => m_npcHeightRate;

    [Header("Time")]
    [SerializeField] Transform sunAnchor;
    [SerializeField] float m_workStartTime = 7.0f;
    [SerializeField] float m_workEndTime = 22.0f;
    [SerializeField] float hourSecondsRatio = 60.0f;
    public float workStartTime => m_workStartTime;
    public float workEndTime => m_workEndTime;
    [field:SerializeField]
    public float currentTime { get; private set; } = 0.0f;
    public bool isWorkTime => currentTime > workStartTime && currentTime < workEndTime;
    [field: SerializeField]
    public int currentDay { get; private set; } = 1;
    public Season currentSeason
    {
        get
        {
            if (currentDay < 4) return Season.Spring;
            if (currentDay < 7) return Season.Summer;
            if (currentDay < 10) return Season.Fall;
            else return Season.Winter;
        }
    }
    public Action onDayChange;
    private void Update()
    {
        currentTime += Time.deltaTime * (1.0f / hourSecondsRatio);
        if (currentTime > 24.0f)
        {
            currentTime = 0.0f;
            currentDay++;
            if (currentDay > 12) currentDay = 1;
            onDayChange?.Invoke();
        }

        sunAnchor.rotation = Quaternion.Euler(-90.0f + currentTime / 24.0f * 360.0f, 30.0f, 0.0f);
    }
    public void SwitchIsland(Island island)
    {
        if (island == currentIsland) return;
    }
}
[System.Serializable]
public enum Season
{
    Spring = 0,
    Summer = 1,
    Fall = 2,
    Winter = 3
}