using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] float flySpeed = 2.0f;
    [SerializeField] float cameraSensitivity = 1.0f;
    [SerializeField] Transform cam;
    [SerializeField] float zoomMin = 0.0f, zoomMax = 10.0f;
    [SerializeField] float zoomSensitivity = 1.0f;
    private void Update()
    {
        Rotate();
        Move();
        Zoom();
    }
    private void Move()
    {
        Vector3 pos = transform.position;

        Vector3 camFront = Camera.main.transform.forward, camRight = Camera.main.transform.right;
        camFront.y = 0.0f; camRight.y = 0.0f;
        camFront.Normalize(); camRight.Normalize();

        pos += (camFront * InputManager.Instance.GetMoveInput().y + camRight * InputManager.Instance.GetMoveInput().x) * moveSpeed * Time.deltaTime;
        pos += Vector3.up * InputManager.Instance.GetFlyInput() * flySpeed * Time.deltaTime;

        if (GameManager.Instance.currentIsland != null)
        {
            Vector3 min = GameManager.Instance.currentIsland.moveBoundsMin;
            Vector3 max = GameManager.Instance.currentIsland.moveBoundsMax;
            pos = new Vector3(Mathf.Clamp(pos.x, min.x, max.x), Mathf.Clamp(pos.y, min.y, max.y), Mathf.Clamp(pos.z, min.z, max.z));
        }

        transform.position = pos;
    }
    void Rotate()
    {
        if (InputManager.Instance.GetCameraMoveInput())
        {
            transform.rotation *= Quaternion.Euler(0, InputManager.Instance.GetCameraMoveValueInput() * cameraSensitivity, 0);
        }
    }
    void Zoom()
    {
        float zoom = cam.transform.localPosition.z;
        zoom += InputManager.Instance.GetCameraZoomInput() * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        cam.transform.localPosition = new Vector3(0, 0, zoom);
    }
}