using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UIManager() => Instance = this;

    [SerializeField] DefaultUI m_defaultUI;
    [SerializeField] BuildUI m_buildUI;
    public DefaultUI defaultUI => m_defaultUI;
    public BuildUI buildUI => m_buildUI;
}