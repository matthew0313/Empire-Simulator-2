using System;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : PlacedMapElement, IWorkplace
{
    [Header("Blueprint")]
    [SerializeField] Structure m_structure;
    public Structure structure => m_structure;
    public ItemIntPair[] ingredients => structure.buildIngredients;
    public int levelRequirement => structure.buildLevelRequirement;
    public int maxWorkers => structure.buildMaxWorkers;
    public float progressRequired => structure.buildProgressRequired;
    public List<NPC> workers { get; } = new();
    public float progress { get; private set; } = 0.0f;
    public Action onProgressChange;
    public void AddProgress(float amount)
    {
        progress += amount;
        if (progress >= progressRequired)
        {
            var tmp = Instantiate(structure, transform.position, transform.rotation);
            placedIsland.PlaceElement(tmp);

            placedIsland.RemoveElement(this);
            Destroy(gameObject);
        }
        else onProgressChange?.Invoke();
    }

    readonly List<Collider> obstructions = new();
    public bool canPlace => !isObstructed && !cannotPlace;
    public bool isObstructed => obstructions.Count > 0;

    PlacementMode m_mode = PlacementMode.Fixed;
    public PlacementMode mode
    {
        get => m_mode;
        set
        {
            if (m_mode == value) return;
            m_mode = value;
            switch (m_mode)
            {
                case PlacementMode.Fixed:
                    RevertMaterial(); break;
                case PlacementMode.CanPlace:
                    SetMaterial(GameManager.Instance.canPlaceMaterial); break;
                case PlacementMode.CannotPlace:
                    SetMaterial(GameManager.Instance.cannotPlaceMaterial); break;
            }
        }
    }
    public bool isPlacing { get; private set; } = false;
    public Action onIsPlacingChange;

    public bool cannotPlace = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!obstructions.Contains(other)) obstructions.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (obstructions.Contains(other)) obstructions.Remove(other);
    }
    public void EnterPlaceMode()
    {
        isPlacing = true;
        onIsPlacingChange?.Invoke();
    }
    public void ExitPlaceMode()
    {
        isPlacing = false;
        onIsPlacingChange?.Invoke();
    }
    private void Update()
    {
        if (!isPlacing)
        {
            mode = PlacementMode.Fixed;
            return;
        }

        if (!canPlace)
        {
            mode = PlacementMode.CannotPlace;
            return;
        }

        mode = PlacementMode.CanPlace;
    }
}
public enum PlacementMode
{
    Fixed,
    CanPlace,
    CannotPlace
}