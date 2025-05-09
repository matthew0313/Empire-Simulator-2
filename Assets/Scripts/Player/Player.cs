using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 moveRangeMin, moveRangeMax;
    [SerializeField] Transform cam;
    [SerializeField] float zoomRate;
    [SerializeField] float camZMin, camZMax;
    // Update is called once per frame
    void Update()
    {
        UpdateCam();
        Move();
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 tmp = new Vector3(Mathf.Abs(moveRangeMin.x - moveRangeMax.x), 5, Mathf.Abs(moveRangeMin.z - moveRangeMax.z));
        Gizmos.DrawCube((moveRangeMin + moveRangeMax) / 2.0f, tmp);
    }
    private void Move()
    {
        Vector3 pos = transform.position;
        Vector3 fwd = Camera.main.transform.forward, right = Camera.main.transform.right;
        fwd.y = 0; fwd.Normalize();
        right.y = 0; right.Normalize();
        pos += (fwd * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")) * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, moveRangeMin.x, moveRangeMax.x);
        pos.z = Mathf.Clamp(pos.z, moveRangeMin.z, moveRangeMax.z);
        transform.position = pos;
    }
    private void UpdateCam()
    {
        if (Input.GetMouseButton(1)) transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
        cam.localPosition = new Vector3(0, 0, Mathf.Clamp(cam.localPosition.z + Input.mouseScrollDelta.y * zoomRate * Time.deltaTime, camZMin, camZMax));
    }
}
