using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;
    public Vector2 GetMoveInput() => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    public float GetFlyInput()
    {
        float tmp = 0.0f;
        if (Input.GetKey(KeyCode.E)) tmp += 1;
        if (Input.GetKey(KeyCode.Q)) tmp -= 1;
        return tmp;
    }
    public bool GetCameraMoveInput() => Input.GetMouseButton(1);
    public float GetCameraMoveValueInput() => Input.GetAxis("Mouse X");
    public float GetCameraZoomInput() => Input.mouseScrollDelta.y;
}