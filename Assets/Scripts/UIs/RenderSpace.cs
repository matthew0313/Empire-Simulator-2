using UnityEngine;
using MEC;
using System.Collections.Generic;

public class RenderSpace : MonoBehaviour
{
    static RenderSpace prefab;
    static List<RenderSpace> renderSpaces;
    static float currentRotation;
    static CoroutineHandle update;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        prefab = Resources.Load<RenderSpace>("RenderSpace");
        renderSpaces = new();
        currentRotation = 0.0f;
    }
    static IEnumerator<float> UpdateRenderSpaces()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(prefab.tickRate);
            currentRotation += prefab.rotationPerSecond * prefab.tickRate;
            foreach (var i in renderSpaces)
            {
                i.OnTick();
            }
        }
    }
    public static RenderSpace Create()
    {
        var tmp = Instantiate(prefab, Vector3.right * (10000.0f + 100.0f * renderSpaces.Count), Quaternion.identity);
        renderSpaces.Add(tmp);
        tmp.OnCreate();
        return tmp;
    }

    [SerializeField] Camera camera;
    [SerializeField] Transform camAnchor;
    [SerializeField] Transform objectAnchor;

    [Header("Output")]
    [SerializeField] RenderTexture textureOrigin;

    [Header("Settings")]
    [SerializeField] float tickRate = 0.25f;
    [SerializeField] float rotationPerSecond = 90.0f;
    public RenderTexture texture { get; private set; }
    void OnCreate()
    {
        if (!update.IsValid) update = Timing.RunCoroutine(UpdateRenderSpaces());
        texture = new RenderTexture(textureOrigin);
        texture.Create();
        camera.targetTexture = texture;
        objectAnchor.rotation = Quaternion.Euler(0, currentRotation, 0);
    }
    void OnTick()
    {
        objectAnchor.rotation = Quaternion.Euler(0, currentRotation, 0);
        camera.Render();
    }
    GameObject renderingObject = null;
    public RenderSpace SetObject(GameObject obj)
    {
        if (renderingObject != null) Destroy(renderingObject);
        obj.transform.SetParent(objectAnchor);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        renderingObject = obj;
        return this;
    }
    public RenderSpace SetCamera(float distance, float height, float angle)
    {
        camera.transform.localPosition = new Vector3(0, 0, -distance);
        camAnchor.transform.localPosition = Vector3.up * height;
        camAnchor.transform.localRotation = Quaternion.Euler(angle, 0, 0);
        return this;
    }
    private void OnDestroy()
    {
        renderSpaces.Remove(this);
    }
}