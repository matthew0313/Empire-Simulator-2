using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using static UnityEditor.PlayerSettings;

public class Freecam : MonoBehaviour
{
    [SerializeField] CinemachineCamera cam;
    float moveSpeed => Settings.cameraMoveSpeed;
    float flySpeed => Settings.cameraFlySpeed;
    float sensitivity => Settings.cameraSensitivity;
    float xRotRange => Settings.xRotRange;
    bool yReverse => Settings.yReverse;

    [Header("Movement Bounds")]
    [SerializeField] Vector3 moveBoundsCenter;
    [SerializeField] Vector3 m_moveBoundsMin, m_moveBoundsMax;
    public Vector3 moveBoundsMin => moveBoundsCenter + m_moveBoundsMin;
    public Vector3 moveBoundsMax => moveBoundsCenter + m_moveBoundsMax;

    float yRot = 0.0f, xRot = 0.0f;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((moveBoundsMax + moveBoundsMin) / 2.0f, moveBoundsMax - moveBoundsMin);
    }
    private void Awake()
    {
        yRot = transform.eulerAngles.y;
        xRot = transform.eulerAngles.x;
    }
    private void Update()
    {
        if (!cam.IsLive) return;
        Rotate();
        Move();
        //Zoom();
    }
    void Rotate()
    {
        xRot += InputManager.Instance.GetCameraRotateInput().y * sensitivity * (yReverse ? -1.0f : 1.0f);
        yRot += InputManager.Instance.GetCameraRotateInput().x * sensitivity;
        xRot = Mathf.Clamp(xRot, -xRotRange, xRotRange);
        transform.rotation = Quaternion.Euler(xRot, yRot, 0.0f);
    }
    void Move()
    {
        Vector3 pos = transform.position;

        Vector3 fwd = transform.forward;
        fwd.y = 0.0f; fwd.Normalize();
        pos += fwd * InputManager.Instance.GetCameraMoveInput().y * moveSpeed * Time.deltaTime;
        pos += transform.right * InputManager.Instance.GetCameraMoveInput().x * moveSpeed * Time.deltaTime;

        pos += Vector3.up * InputManager.Instance.GetFlyInput() * flySpeed * Time.deltaTime;

        Vector3 min = moveBoundsMin;
        Vector3 max = moveBoundsMax;
        pos = new Vector3(Mathf.Clamp(pos.x, min.x, max.x), Mathf.Clamp(pos.y, min.y, max.y), Mathf.Clamp(pos.z, min.z, max.z));

        transform.position = pos;
    }
    /*private void Move()
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
    }*/
}