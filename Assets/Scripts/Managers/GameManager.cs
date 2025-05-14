using System.Collections.Generic;
using UnityEngine;
using MEC;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;
    [SerializeField] Island m_currentIsland;
    public Island currentIsland => m_currentIsland;
    public void SwitchIsland(Island island)
    {
        if (island == currentIsland) return;
    }
}