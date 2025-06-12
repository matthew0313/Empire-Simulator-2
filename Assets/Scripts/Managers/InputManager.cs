using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;
    public Vector2 GetCameraMoveInput() => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    public float GetFlyInput()
    {
        float tmp = 0.0f;
        if (Input.GetKey(KeyCode.E)) tmp += 1;
        if (Input.GetKey(KeyCode.Q)) tmp -= 1;
        return tmp;
    }
    public Vector2 GetCameraRotateInput() => Input.GetMouseButton(1) ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;
    public float GetCameraZoomInput() => Input.mouseScrollDelta.y;
}