using System.Collections.Generic;
using UnityEngine;
using MEC;

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
    public void SwitchIsland(Island island)
    {
        if (island == currentIsland) return;
    }
}