using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraLooker : MonoBehaviour
{
    Camera mainCam;
    bool initialized = false;
    void Init()
    {
        initialized = true;
        mainCam = Camera.main;
    }
    private void OnEnable()
    {
        if (!initialized) Init();
    }
    private void Update()
    {
        transform.forward = mainCam.transform.forward;
    }
}