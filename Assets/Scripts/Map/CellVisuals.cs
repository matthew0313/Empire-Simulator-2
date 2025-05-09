using System.Collections.Generic;
using UnityEngine;

public class CellVisuals : PooledPrefab<CellVisuals>
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject rendered;
    [SerializeField] float shrinkLerpRate;
    public Vector3Int pos { get; private set; }
    public void SetPos(Vector3Int pos)
    {
        this.pos = pos;
        transform.position = MapManager.Instance.CellToWorld(pos);
    }
    readonly int shrinkID = Animator.StringToHash("Shrink");
    float shrink = 1.0f;
    int shrinkStartDist => MapManager.Instance.shrinkStartDist;
    int maxDist => MapManager.Instance.maxDist;
    private void Update()
    {
        Vector3Int pos = MapManager.Instance.WorldToCell(Camera.main.ScreenPointToRay(Input.mousePosition).AimPosY(MapManager.Instance.gridY));
        int dist = Mathf.Abs(this.pos.x - pos.x) + Mathf.Abs(this.pos.z - pos.z);
        float targetShrink = (Mathf.Clamp(dist, shrinkStartDist, maxDist) - shrinkStartDist) / (float)(maxDist - shrinkStartDist);

        if (MapManager.Instance.TryGetCell(this.pos, out CellData data) && data.obstructed == false)
        {
            shrink = Mathf.Lerp(shrink, targetShrink, shrinkLerpRate * Time.deltaTime);
            anim.SetFloat(shrinkID, shrink);
            rendered.SetActive(shrink < 0.99f);
        }
        else rendered.SetActive(false);
    }
    protected override void OnGet()
    {
        base.OnGet();
        shrink = 1.0f;
        anim.SetFloat(shrinkID, shrink);
        rendered.SetActive(false);
    }
}